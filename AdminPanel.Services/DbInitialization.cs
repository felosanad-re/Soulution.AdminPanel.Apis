using AdminPanel.Core;
using AdminPanel.Core.Entities.Identity;
using AdminPanel.Core.Service_Contract;
using Microsoft.AspNetCore.Identity;

namespace AdminPanel.Services
{
    public class DbInitialization : IDbInitialize
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitialization(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task CreateInitializationAsync()
        {
            string[] rolesName = [Roles.SuperAdmin, Roles.Admin, Roles.User];

            foreach (var role in rolesName)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
            // Create Admin
            var user = new ApplicationUser()
            {
                FirstName = "Felo",
                LastName = "Sanad",
                UserName = "Super_Admin",
                Email = "SuperAdmin@Domain.com",
                Address = "Giza"
            };
            var userEmail = await _userManager.FindByEmailAsync(user.Email);
            if (userEmail is null)
            {
                var result = await _userManager.CreateAsync(user, "Admin1234$");
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        throw new Exception(error.Description);
                    }
                }
                await _userManager.AddToRoleAsync(user, Roles.SuperAdmin);
            }
        }
    }
}
