using Documentify.ApplicationCore.Common.Exceptions;
using Documentify.ApplicationCore.Common.Interfaces;
using Documentify.ApplicationCore.Features.Auth.Login;
using Documentify.ApplicationCore.Features.Auth.Register;
using Documentify.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static Documentify.Infrastructure.InfrastructureServiceExtensions;

namespace Documentify.Infrastructure.Identity
{
    public class AuthService(UserManager<ApplicationUser> _userManager,
                            ITokenGenerator _tokenGenerator,
                            IConfiguration _configuration,
                            ILogger<IAuthService> _logger) : IAuthService
    {
        public async Task<LoginCommandResponse> LoginAsync(string usernameOrEmail, string password)
        {
            var user = await _userManager.FindByEmailAsync(usernameOrEmail);
            user ??= await _userManager.FindByNameAsync(usernameOrEmail);

            if (user is null)
                throw new BadRequestException("Invalid credentials.");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
            if (!isPasswordValid)
                throw new BadRequestException("Invalid credentials.");

            if(user.EmailConfirmed == false)
                throw new BadRequestException("Email not confirmed.");

            var expiryMinutes = int.Parse(_configuration[ConfigurationKeys.JwtExpiryMinutes]!);
            return new LoginCommandResponse
            (
                Token: _tokenGenerator.GenerateToken(user.Id, user.Email),
                Expiration: DateTime.UtcNow.AddMinutes(expiryMinutes)
            );
        }
        public async Task<RegisterCommandResponse> RegisterAsync(string username, string email, string password)
        {
            var user = new ApplicationUser
            {
                UserName = username,
                Email = email,
            };
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(",", result.Errors.Select(e => e.Description));
                _logger.LogWarning("User registration failed: {errors}", errors);
                throw new BadRequestException("Registration failed: " + errors);
            }
            var expiryMinutes = int.Parse(_configuration[ConfigurationKeys.JwtExpiryMinutes]!);
            return new(
                token: _tokenGenerator.GenerateToken(user.Id, username),
                expirationDate: DateTime.UtcNow.AddMinutes(expiryMinutes)
             );
        }
    }
}
