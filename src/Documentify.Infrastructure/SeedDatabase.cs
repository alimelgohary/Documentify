using Documentify.ApplicationCore;
using Documentify.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Documentify.Infrastructure
{
    public class SeedDatabase(RoleManager<IdentityRole> _roleManager) : ISeedDatabase
    {
        public async Task Seed()
        {
            await SeedRoles();
        }
        async Task SeedRoles()
        {
            bool rolesExist = await _roleManager.Roles.AnyAsync();
            if (!rolesExist)
            {
                foreach (var role in typeof(Role).GetEnumNames())
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
