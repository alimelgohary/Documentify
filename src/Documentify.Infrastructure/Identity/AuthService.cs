using Documentify.ApplicationCore.Common.Exceptions;
using Documentify.ApplicationCore.Common.Interfaces;
using Documentify.ApplicationCore.Features.Auth.Login;
using Documentify.ApplicationCore.Features.Auth.RefreshToken;
using Documentify.ApplicationCore.Features.Auth.Register;
using Documentify.ApplicationCore.Repository;
using Documentify.Domain.Enums;
using Documentify.Infrastructure.Data;
using Documentify.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static Documentify.Infrastructure.InfrastructureServiceExtensions;

namespace Documentify.Infrastructure.Identity
{
    public class AuthService(UserManager<ApplicationUser> _userManager,
                            SignInManager<ApplicationUser> _signInManager,
                            AppDbContext _context,
                            ITokenGenerator _tokenGenerator,
                            IConfiguration _configuration,
                            ILogger<IAuthService> _logger,
                            IUnitOfWork unitOfWork) : IAuthService
    {
        public async Task<LoginCommandResponse> LoginAsync(string usernameOrEmail, string password)
        {
            var normalizedEmail = _userManager.NormalizeEmail(usernameOrEmail);
            var normalizedName = _userManager.NormalizeName(usernameOrEmail);

            var user = await _userManager.Users
                            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail
                                                   || u.NormalizedUserName == normalizedName);

            if (user is null)
                throw new BadRequestException("Invalid credentials.");

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
            if (signInResult.Succeeded == false)
                throw new BadRequestException("Invalid credentials.");

            if (user.EmailConfirmed == false)
                throw new BadRequestException("Email not confirmed.");

            var roles = (await _userManager.GetRolesAsync(user))
                                .Select(x => Enum.Parse<Role>(x))
                                .ToArray();

            var expiryMinutes = int.Parse(_configuration[ConfigurationKeys.JwtExpiryMinutes]!);
            var expiryMinutesRefresh = int.Parse(_configuration[ConfigurationKeys.JwtRefreshExpiryMinutes]!);
            return new LoginCommandResponse
            (
                AccessToken: _tokenGenerator.GenerateToken(user.Id, user.Email, roles, JwtTokenType.Jwt),
                AccessTokenExpiry: DateTime.UtcNow.AddMinutes(expiryMinutes),
                RefreshToken: _tokenGenerator.GenerateToken(user.Id, user.Email, roles, JwtTokenType.Refresh),
                RefreshTokenExpiry: DateTime.UtcNow.AddMinutes(expiryMinutesRefresh)
            );
        }
        public async Task<RegisterCommandResponse> RegisterAsync(string username, string email, string password, Role[] roles)
        {
            var user = new ApplicationUser
            {
                UserName = username,
                Email = email,
            };

            using var transaction = _context.Database.BeginTransaction();
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                transaction.Rollback();
                var errors = string.Join(",", result.Errors.Select(e => e.Description));
                _logger.LogWarning("User registration failed: {errors}", errors);
                throw new BadRequestException("Registration failed: " + errors);
            }

            var roleRes = await _userManager.AddToRolesAsync(user, roles.Select(x => x.ToString()));
            if (!roleRes.Succeeded)
            {
                transaction.Rollback();
                _logger.LogError(string.Join(",", roleRes.Errors.Select(x => x.Description)));
                throw new BadRequestException("User registration failed duo to unknown error");
            }
            await transaction.CommitAsync();
            return new("Successful registration, please check your email address");
        }

        public async Task<RefreshTokenResponse> RefreshTokenAsync(string oldRefreshToken, CancellationToken cancellationToken)
        {
            _tokenGenerator.ValidateRefreshToken(oldRefreshToken);

            bool IsRevoked = await unitOfWork.Repository<RevokedRefreshToken, int>()
                                                    .ExistsAsync(x => x.Token == oldRefreshToken);

            if (IsRevoked)
                throw new BadRequestException("Refresh token has been revoked.");

            var token = new JwtSecurityTokenHandler().ReadJwtToken(oldRefreshToken);
            var oldRefreshTokenExpiry = token.ValidTo;

            await unitOfWork.Repository<RevokedRefreshToken, int>().AddAsync(
                new RevokedRefreshToken
                {
                    Token = oldRefreshToken,
                    NaturalExpireDate = oldRefreshTokenExpiry
                }, cancellationToken);

            await unitOfWork.CompleteAsync(cancellationToken);

            var expiryMinutes = int.Parse(_configuration[ConfigurationKeys.JwtExpiryMinutes]!);
            var expiryMinutesRefresh = int.Parse(_configuration[ConfigurationKeys.JwtRefreshExpiryMinutes]!);
            
            string userId = token.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                throw new BadRequestException("User not found");

            var roles = (await _userManager.GetRolesAsync(user))
                                .Select(Enum.Parse<Role>)
                                .ToArray();

            return new RefreshTokenResponse
            (
                AccessToken: _tokenGenerator.GenerateToken(user.Id, user.Email, roles, JwtTokenType.Jwt),
                AccessTokenExpiry: DateTime.UtcNow.AddMinutes(expiryMinutes),
                RefreshToken: _tokenGenerator.GenerateToken(user.Id, user.Email, roles, JwtTokenType.Refresh),
                RefreshTokenExpiry: DateTime.UtcNow.AddMinutes(expiryMinutesRefresh)
            );
        }

    }
}
