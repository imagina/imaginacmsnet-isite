﻿using Idata.Data;

namespace Isite.Data.Seeders
{
    public class IsiteSeeder
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<IdataContext>();

                context.Database.EnsureCreated();

                IsiteModuleSeeder.Seed(applicationBuilder);
                CoreModuleSeeder.Seed(applicationBuilder);
            }
        }
    }
}
