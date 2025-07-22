using Microsoft.EntityFrameworkCore;
using PlatformService.Entities;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app, bool isProduction)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>());
            }
        }

        private static void SeedData(AppDbContext context, bool isProduction)
        {
            if (isProduction)
            {
                Console.WriteLine("--> Attemptin to apply migration...")
                try
                {
                    context.Database.Migrate()
                }
                catch (Exception ex)
                {
                    Console.WriteLine("--> Failed to make migrations.")
                }
            }

            if (!context.Platforms.Any())
            {
                Console.WriteLine("--> Seeding Data...");

                context.Platforms.AddRange(
                    new Platform() { Name = "Dot Net", Publisher = "Microsoft", Cost = "Free" },
                    new Platform() { Name = "SQL Server", Publisher = "Microsoft", Cost = "Free" },
                    new Platform() { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundations", Cost = "Free" });

                context.SaveChanges();

            }
            else
            {
                Console.WriteLine("--> Data already exists");
            }
        }

    }
}
