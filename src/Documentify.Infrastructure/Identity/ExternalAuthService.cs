using Documentify.ApplicationCore.Common.Exceptions;
using Documentify.ApplicationCore.Common.Interfaces;
using Documentify.ApplicationCore.Features.Auth.Login;
using Documentify.Domain.Enums;
using Documentify.Infrastructure.Data;
using Documentify.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Security.Claims;
using static Documentify.Infrastructure.InfrastructureServiceExtensions;

namespace Documentify.Infrastructure.Identity
{
    public class ExternalAuthService(SignInManager<ApplicationUser> _signInManager,
                                     UserManager<ApplicationUser> _userManager,
                                     ILogger<ExternalAuthService> _logger,
                                     AppDbContext _context,
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


            if (result.Succeeded)
            {
                var oldUser = await _userManager.FindByEmailAsync(email);
                var roles = (await _userManager.GetRolesAsync(oldUser!))
                                .Select(x => Enum.Parse<Role>(x))
                                .ToArray();
                return new ExternalLoginCommandResponse(
                    AccessToken: tokenGenerator.GenerateToken(userId, email, roles, JwtTokenType.Jwt),
                    RefreshToken: tokenGenerator.GenerateToken(userId, email, roles, JwtTokenType.Refresh),
                    AccessTokenExpiry: DateTime.UtcNow.AddMinutes(expiryMinutes),
                    RefreshTokenExpiry: DateTime.UtcNow.AddMinutes(expiryMinutesRefresh)
                );
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
            };
            using var transaction = _context.Database.BeginTransaction();
            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                transaction.Rollback();
                var errors = string.Join(",", createResult.Errors.Select(e => e.Description));
                _logger.LogWarning("User registration failed: {errors}", errors);
                throw new BadRequestException("Registration failed: " + errors);
            }
            var roleRes = await _userManager.AddToRoleAsync(user, Role.User.ToString());
            if (!roleRes.Succeeded)
            {
                transaction.Rollback();
                _logger.LogError(string.Join(",", roleRes.Errors.Select(x => x.Description)));
                throw new BadRequestException("User registration failed duo to unknown error");
            }
            await transaction.CommitAsync();
            await _userManager.AddLoginAsync(user, info);
            return new ExternalLoginCommandResponse(
                AccessToken: tokenGenerator.GenerateToken(userId, email, [Role.User], JwtTokenType.Jwt),
                RefreshToken: tokenGenerator.GenerateToken(userId, email, [Role.User], JwtTokenType.Refresh),
                AccessTokenExpiry: DateTime.UtcNow.AddMinutes(expiryMinutes),
                RefreshTokenExpiry: DateTime.UtcNow.AddMinutes(expiryMinutesRefresh)
            );
        }
    }
}
