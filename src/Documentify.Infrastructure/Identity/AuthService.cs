using Documentify.ApplicationCore.Common.Exceptions;
using Documentify.ApplicationCore.Common.Interfaces;
using Documentify.ApplicationCore.Features.Auth.Login;
using Documentify.ApplicationCore.Features.Auth.RefreshToken;
using Documentify.ApplicationCore.Features.Auth.Register;
using Documentify.ApplicationCore.Repository;
using Documentify.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using static Documentify.Infrastructure.InfrastructureServiceExtensions;

namespace Documentify.Infrastructure.Identity
{
    public class AuthService(UserManager<ApplicationUser> _userManager,
                            ITokenGenerator _tokenGenerator,
                            IConfiguration _configuration,
                            ILogger<IAuthService> _logger,
                            IUnitOfWork unitOfWork) : IAuthService
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
            var expiryMinutesRefresh = int.Parse(_configuration[ConfigurationKeys.JwtRefreshExpiryMinutes]!);
            return new LoginCommandResponse
            (
                AccessToken: _tokenGenerator.GenerateToken(user.Id, user.Email, JwtTokenType.Jwt),
                AccessTokenExpiry: DateTime.UtcNow.AddMinutes(expiryMinutes),
                RefreshToken: _tokenGenerator.GenerateToken(user.Id, user.Email, JwtTokenType.Refresh),
                RefreshTokenExpiry: DateTime.UtcNow.AddMinutes(expiryMinutesRefresh)
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
            return new("Successful registration, please check your email address");
        }

        public async Task<RefreshTokenResponse> RefreshTokenAsync(string oldRefreshToken, CancellationToken cancellationToken)
        {
            _tokenGenerator.ValidateRefreshToken(oldRefreshToken);

            var expiryMinutes = int.Parse(_configuration[ConfigurationKeys.JwtExpiryMinutes]!);
            var expiryMinutesRefresh = int.Parse(_configuration[ConfigurationKeys.JwtRefreshExpiryMinutes]!);

            var oldRefreshTokenExpiry = new JwtSecurityTokenHandler().ReadJwtToken(oldRefreshToken).ValidTo;

            bool IsRevoked = await unitOfWork.Repository<RevokedRefreshToken, int>()
                                    .ExistsAsync(x => x.Token == oldRefreshToken);

            if(IsRevoked)
                throw new BadRequestException("Refresh token has been revoked.");

            await unitOfWork.Repository<RevokedRefreshToken, int>().AddAsync(
                new RevokedRefreshToken
                {
                    Token = oldRefreshToken,
                    NaturalExpireDate = oldRefreshTokenExpiry
                }, cancellationToken);

            await unitOfWork.CompleteAsync(cancellationToken);

            return new RefreshTokenResponse
            (
                AccessToken: _tokenGenerator.GenerateToken(oldRefreshToken, JwtTokenType.Jwt),
                AccessTokenExpiry: DateTime.UtcNow.AddMinutes(expiryMinutes),
                RefreshToken: _tokenGenerator.GenerateToken(oldRefreshToken, JwtTokenType.Refresh),
                RefreshTokenExpiry: DateTime.UtcNow.AddMinutes(expiryMinutesRefresh)
            );
        }

    }
}
