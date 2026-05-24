using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PMS.Application.DTOs;
using PMS.Application.Interfaces;
using PMS.Domain.Constants;
using PMS.Domain.Entities;
using PMS.Domain.Enums;
using PMS.Infrastructure.Persistence;

namespace PMS.Infrastructure.Repositories.Auth
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly AppDbContext _db;

        public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager, AppDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }

        public async Task<UserInfoDto?> CreateUserAsync(string userName, string email, string password, string? role = null)
        {
            var user = new ApplicationUser(userName, email);
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded) return null;

            var normalizedRole = NormalizeRole(role);
            var roleExists = await _roleManager.RoleExistsAsync(normalizedRole);
            if (!roleExists)
            {
                // if role does not exists it will create role 
                var roleResult = await _roleManager.CreateAsync(new IdentityRole<Guid>(normalizedRole));
                if (!roleResult.Succeeded) return null;
            }

            var addRoleResult = await _userManager.AddToRoleAsync(user, normalizedRole);
            if (!addRoleResult.Succeeded) return null;

            if (string.Equals(normalizedRole, RoleNames.Employee, StringComparison.OrdinalIgnoreCase))
            {
                /* after user creation in aspnetusers table. i am creating employee becuase registraion is only for employee .
                 * admin is seeded while first time when api will run. 
                aslo you can configure in database as well for admin setup once db is created.*/
                var exists = await _db.Employees.AnyAsync(e => e.UserId == user.Id);
                if (!exists)
                {
                    var employee = new Employee(user.Id, user.UserName ?? string.Empty, user.Email ?? string.Empty);
                    await _db.Employees.AddAsync(employee, CancellationToken.None);
                    await _db.SaveChangesAsync(CancellationToken.None);
                }
            }

            var roles = await _userManager.GetRolesAsync(user);
            return new()
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Roles = roles ?? Enumerable.Empty<string>()
            };
        }

        public async Task<UserInfoDto?> ValidateUserAsync(string userName, string password)
        {
            // validating user 
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null) return null;
            var valid = await _userManager.CheckPasswordAsync(user, password);
            if (!valid) return null;
            var roles = await _userManager.GetRolesAsync(user);
            return new()
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Roles = roles ?? Enumerable.Empty<string>()
            };
        }

        private static string NormalizeRole(string? role)
        {
            
            if (string.IsNullOrWhiteSpace(role))
            {
                return RoleNames.Employee;
            }

            if (Enum.TryParse<UserRole>(role, true, out var parsedRole))
            {
                return parsedRole.ToString();
            }

            throw new InvalidOperationException("Invalid role. Allowed roles: Admin, Employee.");
        }
    }
}
