using ChopDeck.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ChopDeck.Services.Impl
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleService(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task CreateRolesAsync()
        {
            try
            {
                var roles = new[] { "RESTAURANT", "CUSTOMER", "DRIVER" };

                foreach (var role in roles)
                {
                    bool roleExist = await _roleManager.RoleExistsAsync(role);
                    Console.WriteLine($"Checking role: {role}, Exists: {roleExist}");

                    if (!roleExist)
                    {
                        var result = await _roleManager.CreateAsync(new IdentityRole(role));
                        if (!result.Succeeded)
                        {
                            Console.WriteLine($"Error creating role {role}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateRolesAsync: {ex.Message}");
            }
        }

    }
}
