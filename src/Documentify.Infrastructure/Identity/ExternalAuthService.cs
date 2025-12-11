using Documentify.ApplicationCore.Common.Interfaces;
using Documentify.ApplicationCore.Features.Auth.Login;
using Documentify.Domain.Enums;
using Documentify.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using static Documentify.Infrastructure.InfrastructureServiceExtensions;

namespace Documentify.Infrastructure.Identity
{
    public class ExternalAuthService(SignInManager<ApplicationUser> _signInManager,
                                     UserManager<ApplicationUser> _userManager,
                                     ITokenGenerator tokenGenerator,
                                     IConfiguration _configuration) : IExternalAuthService
    {
        public async Task<ExternalLoginCommandResponse> LoginOrRegister(ExternalLoginProvider externalLoginProvider)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                throw new Exception("Error loading external login information.");

            string email = info.Principal.FindFirstValue(ClaimTypes.Email)!;
            string userId = info.Principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            
            var expiryMinutes = int.Parse(_configuration[ConfigurationKeys.JwtExpiryMinutes]!);
            var expiryMinutesRefresh = int.Parse(_configuration[ConfigurationKeys.JwtRefreshExpiryMinutes]!);
            
             var response = new ExternalLoginCommandResponse(
                    AccessToken: tokenGenerator.GenerateToken(userId, email, JwtTokenType.Jwt),
                    RefreshToken: tokenGenerator.GenerateToken(userId, email, JwtTokenType.Refresh),
                    AccessTokenExpiry: DateTime.UtcNow.AddMinutes(expiryMinutes),
                    RefreshTokenExpiry: DateTime.UtcNow.AddMinutes(expiryMinutesRefresh)
                    );
            if (result.Succeeded)
                return response;

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
            return response;
        }
    }
}
