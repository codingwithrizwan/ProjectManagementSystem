using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PMS.Domain.Constants;
using PMS.Domain.Entities;

namespace PMS.Infrastructure.Auth
{

    // This class is responsible for seeding the initial roles and admin user into the Identity system.
    // when ever application starts, it checks if the required roles (Admin) exist, and if not, it creates them.
    // It also checks for the existence of a seeded admin user (based on configuration) and
    public static class IdentitySeederExtensions
    {
        public static async Task SeedIdentityAsync(this IServiceProvider services, IConfiguration configuration)
        {
            using var scope = services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            await EnsureRoleAsync(roleManager, RoleNames.Admin);
            await EnsureRoleAsync(roleManager, RoleNames.Employee);
           // Seed admin user if configured in appsettings.json under "AdminSeed" section
            var seedSection = configuration.GetSection("AdminSeed");
            var userName = seedSection["UserName"];
            var email = seedSection["Email"];
            var password = seedSection["Password"];

            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return;
            }
            
            var admin = await userManager.FindByNameAsync(userName);
            if (admin == null)
            {
                admin = new ApplicationUser(userName, email);
                var createResult = await userManager.CreateAsync(admin, password);
                if (!createResult.Succeeded)
                {
                    throw new InvalidOperationException("Failed to create seeded admin user.");
                }
            }

            if (!await userManager.IsInRoleAsync(admin, RoleNames.Admin))
            {
                var addRoleResult = await userManager.AddToRoleAsync(admin, RoleNames.Admin);
                if (!addRoleResult.Succeeded)
                {
                    throw new InvalidOperationException("Failed to assign Admin role to seeded user.");
                }
            }
        }

        private static async Task EnsureRoleAsync(RoleManager<IdentityRole<Guid>> roleManager, string roleName)
        {
            if (await roleManager.RoleExistsAsync(roleName))
            {
                return;
            }

            var roleResult = await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException($"Failed to create admin role '{roleName}'.");
            }
        }
    }
}
