using ChopDeck.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ChopDeck.Services
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
            var roles = new[] { "RESTAURANT", "CUSTOMER", "DRIVER" };

            foreach (var role in roles)
            {
                var roleExist = await _roleManager.RoleExistsAsync(role);
                if (!roleExist)
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
