using GymSystem.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Data.DataSeeds
{
    public static class IdentityDataSeed
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager,
            ILogger logger, CancellationToken ct = default)
        {
            try
            {
                bool HasUsers = userManager.Users.Any();
                bool HasRole = roleManager.Roles.Any();

                if (HasUsers && HasRole) return;

                if (!HasRole)
                {
                    var roles = new List<IdentityRole>()
                    {
                        new IdentityRole()  { Name = "SuperAdmin"},
                        new IdentityRole()  { Name = "Admin" },
                    };

                    foreach (var role in roles.Select(r => r.Name))
                    {
                        if(!await roleManager.RoleExistsAsync(role))
                        {
                            var roleResult = await roleManager.CreateAsync(new IdentityRole(role));
                            if(!roleResult.Succeeded) logger.LogError($"Failed to create role: {role}. Errors: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                        }
                    }
                }

                if (!HasUsers)
                {
                    var mainUser = new ApplicationUser()
                    {
                        FirstName = "Ahmed",
                        LastName = "Hassan",
                        UserName = "ahmedhassan",
                        Email = "ahmedhassan@example.com",
                        PhoneNumber = "1234567890",
                    };
                   var userResult =  await userManager.CreateAsync(mainUser, "P@ssw0rd");
                    await userManager.AddToRoleAsync(mainUser, "SuperAdmin");

                    if(!userResult.Succeeded)
                    {
                        logger.LogError($"Failed to create user: {mainUser.UserName}. Errors: {string.Join(", ", userResult.Errors.Select(e => e.Description))}");
                        return;
                    }
                }
                return;
            }
            catch (Exception ex)
            {

                logger.LogError(ex, "An error occurred while seeding identity data.");
                throw;
            }
        }
    }
}
