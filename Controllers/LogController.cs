using Core;
using Core.Exceptions;
using Core.Interfaces;
using Ihelpers.Interfaces;
using Iprofile.Helpers;
using Isite.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Isite.Controllers
{
    [Authorize]
    [Route("api/isite/v1/logs")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ILogRepository _repositoryBase;
        private readonly IModuleRepository moduleRepository;
        private readonly ICacheBase cache;
        private readonly IHttpContextAccessor _currentContext;
        public LogController(ILogRepository repository, IModuleRepository moduleRepository, ICacheBase cacheBase, IHttpContextAccessor currentContext)
        {
            _repositoryBase = repository;
            moduleRepository = moduleRepository;
            cache = cacheBase;
            _currentContext = currentContext;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index([FromQuery] UrlRequestBase? urlRequestBase)
        {


            int status = 200;
            dynamic response;
            dynamic meta = new object();
            try
            { //Set the user!
                urlRequestBase.currentContextUser = AuthHelper.AuthUser(_currentContext);
                //parser
                await urlRequestBase.Parse(this);


                //set User 

                //repository
                response = await _repositoryBase.GetItemsBy(urlRequestBase);

                //get meta before transform, because meta will be lost once object is transformed
                meta = await ResponseBase.GetMeta(response);



            }
            catch (ExceptionBase ex)
            {
                return StatusCode(ex.CodeResult, ex.CreateResponseFromException());
            }
            //reponse


            return StatusCode(status, await ResponseBase.Response(response, meta));

        }

        [HttpGet("{criteria}")]
        public virtual async Task<IActionResult> Show(string? criteria, [FromQuery] UrlRequestBase? urlRequestBase)
        {
            int status = 200;
            object? response;
            try
            {
                urlRequestBase.currentContextUser = AuthHelper.AuthUser(_currentContext);
                await urlRequestBase.Parse(this);


                urlRequestBase.criteria = criteria;

                response = await _repositoryBase.GetItem(urlRequestBase);

            }
            catch (ExceptionBase ex)
            {
                return StatusCode(ex.CodeResult, ex.CreateResponseFromException());
            }
            return StatusCode(status, await ResponseBase.Response(response));
        }

    }
}
