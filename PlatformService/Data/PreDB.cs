using System;
using Microsoft.EntityFrameworkCore;

namespace PlatformService.Data
{
    public static class PreDB
    {
        public static void PrepPopulation(IApplicationBuilder app, bool isProd)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDBContext>(), isProd);
            }
        }

        private static void SeedData(AppDBContext? context, bool isProd)
        {
            if (isProd)
            {
                Console.WriteLine("--> Attempting to apply migrations ...");
                try
                {
                    context?.Database.Migrate();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"--> Attempting to apply migrations FAILED: {e.Message}.");
                }
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }



            if (!context.Platforms.Any())
            {
                Console.WriteLine("--> Seeding Data");
                context.Platforms.AddRange(
                    new Models.Platform() { Name = "DotNet", Publisher = "Microsoft", Cost = "Free" },
                    new Models.Platform() { Name = "Sql Server Express", Publisher = "Microsoft", Cost = "Free" },
                    new Models.Platform() { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
                );
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("DB already has data");
            }
        }
    }
}