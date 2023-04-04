using Azure.Core;
using Core;
using Core.Exceptions;
using Idata.Data;
using Ihelpers.Helpers;
using Iprofile.Helpers;
using Isite.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Isite.Controllers
{
    [Authorize]
    [Route("api/isite/v1/export")]
    [ApiController]
    public class ExportController : ControllerBase
    {
        IExportRepository _exportRepository;
        IServer _iserver;
        IdataContext _dbContext;
        public dynamic _authUser;

        public ExportController(IExportRepository reportRepository, IServer iserver, IHttpContextAccessor currentContext, IdataContext dbContext)
        {
            _exportRepository = reportRepository;
            _iserver = iserver;
            _authUser = AuthHelper.AuthUser(currentContext);
            _dbContext = dbContext;
        }
        [Authorize]
        [HttpGet("")]
        public async Task<IActionResult> Show([FromQuery] UrlRequestBase urlRequestBase)
        {

            try
            {
                Dictionary<string, object>?[] response;

                int status = 200;
                await urlRequestBase.Parse(this);

                string? filename = JObjectHelper.GetJObjectValue<string>(urlRequestBase.InternalExportParams(), "fileName");


				if (filename == "report")
				{
                    var reportId = Convert.ToInt32(urlRequestBase.GetFilter("reportId"));

					var report = _dbContext.Reports.Include("reportType").Where( rep => rep.id == reportId).FirstOrDefault();

                    filename += $"{report.reportType.name.ToLower().Replace(" ", "_").Trim()}{report.id}_{urlRequestBase.currentContextUser.id}";

				}
				else
				{
					filename += $"{JWTHelper.getJWTTokenClaim(urlRequestBase.currentContextToken, "UserId")}";
				}

                urlRequestBase.filename = filename;

                var addresses = _iserver.Features.Get<IServerAddressesFeature>();

                response = await _exportRepository.GetExport(urlRequestBase);

                return Ok(response);




            }
            catch (ExceptionBase ex)
            {
                return StatusCode(ex.CodeResult, ex.CreateResponseFromException());
            }



        }


        [HttpGet("{filename}")]
        public virtual async Task<IActionResult> Export(string? filename, [FromQuery] UrlRequestBase? urlRequestBase)
        {
            try
            {
                await urlRequestBase.Parse(this);


                urlRequestBase.currentContextUser = _authUser;
                string? fileFormat = JObjectHelper.GetJObjectValue<string>(urlRequestBase.InternalExportParams(), "fileFormat");
                //string? filename = JObjectHelper.GetJObjectValue<string>(urlRequestBase.InternalExportParams(), "fileName");


                urlRequestBase.filename = filename;

                byte[]? response = await _exportRepository.DownloadExport(urlRequestBase);

                string contentType = !filename.Contains(".pdf") ? "text/csv" : "application/pdf";



                return File(response, contentType, filename);

            }
            catch (ExceptionBase ex)
            {
                return StatusCode(ex.CodeResult, ex.CreateResponseFromException());
            }

        }

    }
}
