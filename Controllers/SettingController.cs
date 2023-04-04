using Core;
using Core.Transformers;
using Isite.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Isite.Controllers
{

    [Route("api/isite/v1/site")]
    [ApiController]
    public class SettingController : ControllerBase
    {
        private readonly ISettingRepository repository;
        private readonly IModuleRepository moduleRepository;
        private readonly IHttpContextAccessor _currentContext;
        public SettingController(ISettingRepository repository, IModuleRepository moduleRepository, IHttpContextAccessor currentContext)
        {
            this.repository = repository;
            this.moduleRepository = moduleRepository;
            _currentContext = currentContext;
        }

        [HttpGet("settings")]
        public async Task<IActionResult> Index([FromQuery] UrlRequestBase? urlRequestBase)
        {

            await urlRequestBase.Parse(this);
            urlRequestBase.currentContextUser = Iprofile.Helpers.AuthHelper.AuthUser(_currentContext);
            dynamic modulesTransformed;

            //Getting the modules in db
            dynamic modules = await moduleRepository.GetItemsBy(new UrlRequestBase());

            //transforming modules

            try
            {
                modulesTransformed = await TransformerBase.TransformCollection(modules);
            }
            catch 
            {
                throw new Exception("Unable to parse module configurations, bad jsonformat");
            }
           

            JArray modulesEnabled = new JArray();
            JArray siteSettings = new JArray();

            //unifying module's settings and transforming each module in JObject
            foreach (var module in modulesTransformed)
            {
                modulesEnabled.Add(JObject.FromObject(module));

                if (module["settings"] != null)
                {
                    foreach (var setting in module["settings"])
                    {
                        JObject xSetting = JObject.FromObject(setting);
                        siteSettings.Add(xSetting.SelectToken("Value"));
                    }
                }

            }

            //creating the final reponse
            JObject response = new JObject(
                new JProperty("data", new JObject(
                    new JProperty("availableLocales", new JArray(
                        new JObject(
                            new JProperty("name", "English"),
                            new JProperty("script", "Latn"),
                            new JProperty("native", "English"),
                            new JProperty("iso", "en")
                            )
                        )),
                    new JProperty("availableThemes", new JObject()),
                    new JProperty("defaultLocale", "en"),
                    new JProperty("modulesEnabled", modulesEnabled),
                    new JProperty("siteSettings", siteSettings)
                    )));


            return Ok(response.ToString());

        }

        [HttpGet("permissions")]
        public async Task<IActionResult> Permissions([FromQuery] UrlRequestBase? requestBase)
        {


            dynamic modulesTransformed;
            //parser
            await requestBase.Parse();
            requestBase.currentContextUser = Iprofile.Helpers.AuthHelper.AuthUser(_currentContext);
            //Verify the filters that are present in requestBase
            if (requestBase.InternalFilter() != null)
            {
                //Try get the search filter
                var configNameFilter = (requestBase.InternalFilter().SelectToken("configName"))?.ToString();

                if (configNameFilter != null)
                    return Ok();

            }
            //Getting the modules in db
            dynamic modules = await moduleRepository.GetItemsBy(new UrlRequestBase() { currentContextUser = requestBase.currentContextUser });

            //transforming modules
            modulesTransformed = await TransformerBase.TransformCollection(modules);

            JObject modulesEnabled = new JObject();
            JArray siteConfigs = new JArray();

            //unifying module's configs and transforming each module in JObject
            foreach (var module in modulesTransformed)
            {
                modulesEnabled.Add(new JProperty(module["name"], module["permissions"] != null ? JObject.FromObject(module["permissions"]) : null));
            }

            //creating the final reponse
            JObject response = new JObject(
                new JProperty("data", modulesEnabled));


            return Ok(response.ToString());

        }

    }
}
