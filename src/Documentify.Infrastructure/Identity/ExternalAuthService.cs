using Documentify.ApplicationCore.Common.Interfaces;
using Documentify.Domain.Enums;
using Documentify.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Documentify.Infrastructure.Identity
{
    public class ExternalAuthService(SignInManager<ApplicationUser> _signInManager,
                                     UserManager<ApplicationUser> _userManager,
                                     ITokenGenerator tokenGenerator) : IExternalAuthService
    {
        public async Task<string> LoginOrRegister(ExternalLoginProvider externalLoginProvider)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                throw new Exception("Error loading external login information.");

            string email = info.Principal.FindFirstValue(ClaimTypes.Email)!;
            string userId = info.Principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
                return tokenGenerator.GenerateToken(userId, email, JwtTokenType.Jwt);

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
            };

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
                throw new Exception("Error creating user");
                
            await _userManager.AddLoginAsync(user, info);
            return tokenGenerator.GenerateToken(userId, email, JwtTokenType.Jwt);
        }
    }
}
