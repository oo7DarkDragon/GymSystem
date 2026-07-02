using GymSystem.DAL.Context;
using GymSystem.DAL.Data.DataSeeds;
using GymSystem.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GymSystem
{
    public static class ProgramExtensions
    {
        public static async Task MigrationAndSeedAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbcontext = scope.ServiceProvider.GetRequiredService<GymDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var RoleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var pending = await dbcontext.Database.GetPendingMigrationsAsync();

            if (pending.Any())
            {
                logger.LogInformation($"Applying {pending.Count()} Pending Migration....");
                await dbcontext.Database.MigrateAsync();
            }

            var seedPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "Files");

            await GymDataSeed.SeedAsync(dbcontext, seedPath, logger);
            await IdentityDataSeed.SeedAsync(RoleManager, UserManager, logger);

        }
    }
}
