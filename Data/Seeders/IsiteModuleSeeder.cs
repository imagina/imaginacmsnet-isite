﻿using Idata.Data;
using Idata.Data.Entities;
using Idata.Data.Entities.Isite;
using Isite.Config;
using Newtonsoft.Json;

namespace Isite.Data.Seeders
{
    public class IsiteModuleSeeder
    {

        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<IdataContext>();

                context.Database.EnsureCreated();

                object values = new
                {
                    name = "Isite",
                    alias = "isite",
                    permissions = Permissions.GetPermissions(),
                    settings = Settings.GetSettings(),
                    enabled = true,
                    priority = 1,
                    configs = Configs.GetConfigs(),
                    cms_pages = CmsPages.GetCmsPages(),
                    cms_sidebar = CmsSidebar.GetCmsSidebar()
                };


                Module? module = context.Modules.Where(m => m.alias == "isite").FirstOrDefault();

                if (module == null)
                {
                    module = JsonConvert.DeserializeObject<Module>(JsonConvert.SerializeObject(values));
                    context.Modules.Add(module);
                    context.SaveChanges();
                    module = context.Modules.Where(m => m.alias == "isite").FirstOrDefault();
                    module.translations = new List<ModuleTranslation>() {
                        new ModuleTranslation()
                            {
                                locale = "en",
                                title = "Site"
                            }
                        };

                }
                else
                {
                    context.Entry(module).CurrentValues.SetValues(values);
                }



                context.SaveChanges();

            }
        }


    }
}
