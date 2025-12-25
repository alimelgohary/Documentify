using Documentify.ApplicationCore;
using Documentify.Domain.Enums;
using Documentify.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Documentify.Infrastructure
{
    public class SeedDatabase(RoleManager<IdentityRole> _roleManager,
        IHostEnvironment env,
        AppDbContext _context) : ISeedDatabase
    {
        public async Task Seed()
        {
            if(env.EnvironmentName == "Testing")
                await _context.Database.EnsureCreatedAsync();
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
