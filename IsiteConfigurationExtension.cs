using Idata.Data;
using Isite.Repositories;
using Isite.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;

namespace Isite
{

    public static class IsiteServiceProvider
    {


        public static WebApplicationBuilder? Boot(WebApplicationBuilder? builder)
        {

            builder.Services.AddControllers().ConfigureApplicationPartManager(o =>
            {

                o.ApplicationParts.Add(new AssemblyPart(typeof(IsiteServiceProvider).Assembly));
            });

            //Repositories
            builder.Services.AddTransient<ISettingRepository, SettingRepository>();
            builder.Services.AddTransient<IModuleRepository, ModuleRepository>();
            builder.Services.AddTransient<IExportRepository, ExportRepository>();
            builder.Services.AddTransient<ILogRepository, LogRepository>();
            builder.Services.AddScoped<IRevisionRepository, RevisionRepository>();
            return builder;

        }
    }

}
