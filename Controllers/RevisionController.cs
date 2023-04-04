
using Core;
using Core.Controllers;
using Core.Exceptions;
using Core.Transformers;
using Idata.Entities.Isite;
using Iprofile.Helpers;
using Isite.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Isite.Controllers
{
    [Authorize]
    [Route("api/isite/v1/revisions")]
    [ApiController]
    public class RevisionController : ControllerBase<Revision>
    {
        public IRevisionRepository _repository { get; }
        public RevisionController(IRevisionRepository repositoryBase, IHttpContextAccessor currentContext) : base(repositoryBase, AuthHelper.AuthUser(currentContext))
        {
            _repository = repositoryBase;
        }
       
        public override async Task<IActionResult> Show(string? criteria, [FromQuery] UrlRequestBase? urlRequestBase)
        {
            int status = 200;
            dynamic? response = null;
            try
            {
                // Parse the URL request base
                await urlRequestBase.Parse(this);

                // Set the criteria
                urlRequestBase.criteria = criteria;
                
                // Get the item from the repository
                response = await _repositoryBase.GetItem(urlRequestBase);

                // Transform the item
                response = await TransformerBase.TransformItem(response, _cache, userTimezone: urlRequestBase.getRequestTimezone());

                var filterEntity = urlRequestBase.GetFilter("entity");
                if (filterEntity != "id") {

                    switch (filterEntity) {
                        case "newValue":
                        default:
                            response = await TransformerBase.ToClassInCache(response["newValue"].ToString(), response["revisionableType"].ToString());
                            break;

                        case "oldValue":
                            response = await TransformerBase.ToClassInCache(response["oldValue"].ToString(), response["revisionableType"].ToString());
                            break;
                    }                    
                }               
            }
            catch (ExceptionBase ex)
            {
                // Return the code result and exception response
                return StatusCode(ex.CodeResult, ex.CreateResponseFromException());
            }
            // Return the status code and response
            return StatusCode(status, await ResponseBase.Response(response));
        }
    }    
}
