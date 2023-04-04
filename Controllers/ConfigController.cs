using Core;
using Core.Transformers;
using Idata.Data.Entities;
using Idata.Data.Entities.Isite;
using Isite.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Isite.Controllers
{


    [Route("api/isite/v1/configs")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly ISettingRepository repository;
        private readonly IModuleRepository moduleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ConfigController(ISettingRepository repository, IModuleRepository moduleRepository, IHttpContextAccessor currentContext)
        {
            this.repository = repository;
            this.moduleRepository = moduleRepository;
            _httpContextAccessor = currentContext;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index([FromQuery] UrlRequestBase? requestBase)
        {

            dynamic modulesTransformed;
            //parser
            await requestBase.Parse();


            requestBase.currentContextUser = Iprofile.Helpers.AuthHelper.AuthUser(_httpContextAccessor);
            JObject response;

            //initialize moduleRequest
            UrlRequestBase moduleRequest = new UrlRequestBase();

            //possible filter named configName 
            string? configNameFilter = null;
            string[]? splitConfigNameFilter = null;

            //Verify the filters that are present in requestBase: added validation if the frontend send a filter attribute the endpoint will be return empty for now
            if (requestBase.InternalFilter() != null)
            {
                //Try get the search filter
                configNameFilter = requestBase.GetFilter("configName");

                //if isset the filter named configName
                if (configNameFilter != null)
                {

                    //split the value to get the moduleName and the specific config needed
                    // example moduleName.configFile.configKey[.configKey...]
                    splitConfigNameFilter = configNameFilter.Split(".");

                    //seeding the filter by module name
                    moduleRequest.filter = "{field:'name'}";
                    moduleRequest.criteria = splitConfigNameFilter[0];
                    moduleRequest.currentContextUser = requestBase.currentContextUser;
                    //parsing moduleRequest variables
                    await moduleRequest.Parse();

                    //Getting the modules in db
                    Module module = await moduleRepository.GetItem(moduleRequest);
                    JObject moduleJObject = JObject.Parse(JsonConvert.SerializeObject(module));
                    string? configsString = moduleJObject.SelectToken("configs")?.ToString();

                    JObject configs = JObject.Parse(configsString);

                    //replacing the first value because the First value is the Module Name
                    string configNameToFind = configNameFilter.Replace(splitConfigNameFilter[0] + ".", String.Empty);

                    //removing the string "config." because all the configs are already getted in the JObject configs 
                    configNameToFind = configNameToFind.Replace("config.", String.Empty);

                    //getting the object by the string of the configs required in the filter
                    string? finalConfig = configs.SelectToken(configNameToFind)?.ToString();

                    //if the finalConfig is null needs to scape the data response
                    if (finalConfig != null)
                    {

                        //creating the final reponse
                        response = new JObject(
                            new JProperty("data", JsonConvert.DeserializeObject(finalConfig)));

                        return Ok(response.ToString());
                    }
                    else
                    {
                        return Ok();
                    }

                }

                return Ok();
            }
            //Getting the modules in db
            dynamic modules = await moduleRepository.GetItemsBy(moduleRequest);

            //transforming modules
            modulesTransformed = await TransformerBase.TransformCollection(modules);


            JObject modulesEnabled = new JObject();
            JArray siteConfigs = new JArray();



            //unifying module's configs and transforming each module in JObject
            foreach (var module in modulesTransformed)
            {
                modulesEnabled.Add(new JProperty(module["name"], module["configs"] != null ? JObject.FromObject(module["configs"]) : null));
            }



            //creating the final reponse
            response = new JObject(
                new JProperty("data", modulesEnabled));


            return Ok(response.ToString());

        }

    }
}
