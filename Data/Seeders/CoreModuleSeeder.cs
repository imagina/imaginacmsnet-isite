using Idata.Data;
using Idata.Data.Entities;
using Idata.Data.Entities.Isite;
using Newtonsoft.Json;

namespace Isite.Data.Seeders
{
    public class CoreModuleSeeder
    {

        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<IdataContext>();

                context.Database.EnsureCreated();

                object values = new
                {
                    name = "Core",
                    alias = "core",
                    permissions = "{'core.sidebar':{'group':'core::sidebar.show group'}}",
                    settings = "{'core::site-name':{'description':'','default':'My Site','view':'settingField','translatable':true,'id':18,'name':'core::site-name','plainValue':'My Site','isTranslatable':true,'created_at':'2021-11-25T21:36:20.000000Z','updated_at':'2021-11-25T21:36:20.000000Z','value':'My Site','translations':[{'id':1,'setting_id':18,'locale':'en','value':'My Site'},{'id':4,'setting_id':18,'locale':'es','value':'My Site'}]},'core::site-name-mini':{'description':'','view':'settingField','translatable':true,'id':19,'name':'core::site-name-mini','isTranslatable':true,'created_at':'2021-11-25T21:36:20.000000Z','updated_at':'2021-11-25T21:36:20.000000Z','value':'null','translations':[{'id':2,'setting_id':19,'locale':'en','value':'null'},{'id':5,'setting_id':19,'locale':'es','value':'null'}]},'core::site-description':{'description':'','view':'settingField','translatable':true,'id':20,'name':'core::site-description','isTranslatable':true,'created_at':'2021-11-25T21:36:20.000000Z','updated_at':'2021-11-25T21:36:20.000000Z','value':'null','translations':[{'id':3,'setting_id':20,'locale':'en','value':'null'},{'id':6,'setting_id':20,'locale':'es','value':'null'}]},'core::locales':{'description':'','default':'[]','view':'settingField','translatable':false,'id':2,'name':'core::locales','plainValue':['en'],'isTranslatable':false,'created_at':'2021-11-25T21:24:33.000000Z','updated_at':'2022-03-18T23:06:47.000000Z','value':['en'],'translations':[]},'core::template':{'description':'','view':'settingField','translatable':false,'id':1,'name':'core::template','plainValue':'ImaginaTheme','isTranslatable':false,'created_at':'2021-11-25T21:24:33.000000Z','updated_at':'2021-11-25T21:36:20.000000Z','value':'ImaginaTheme','translations':[]},'core::analytics-script':{'description':'','view':'settingField','translatable':false,'id':17,'name':'core::analytics-script','isTranslatable':false,'created_at':'2021-11-25T21:36:20.000000Z','updated_at':'2021-11-25T21:36:20.000000Z','translations':[]}}",
                    enable = true,
                    priority = 1,
                    cms_pages = "",
                    cms_sidebar = ""

                };



                Module? module = context.Modules.Where(m => m.alias == "core").FirstOrDefault();

                if (module == null)
                {
                    module = JsonConvert.DeserializeObject<Module>(JsonConvert.SerializeObject(values));
                    context.Modules.Add(module);
                    context.SaveChanges();
                    module = context.Modules.Where(m => m.alias == "core").FirstOrDefault();
                    module.translations = new List<ModuleTranslation>() {
                    new ModuleTranslation()
                        {
                            locale = "en",
                            title = "Core"
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
