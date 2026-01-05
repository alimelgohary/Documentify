using Documentify.ApplicationCore.Common.Interfaces;
using Documentify.ApplicationCore.Features;
using Documentify.ApplicationCore.Features.Auth.ConfirmEmail;
using Documentify.ApplicationCore.Features.Auth.Login;
using Documentify.ApplicationCore.Features.Auth.RefreshToken;
using Documentify.ApplicationCore.Features.Auth.Register;
using Documentify.ApplicationCore.Mail;
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
using System.Text.Encodings.Web;
using static Documentify.Infrastructure.InfrastructureServiceExtensions;

namespace Documentify.Infrastructure.Identity
{
    public class AuthService(UserManager<ApplicationUser> _userManager,
                            SignInManager<ApplicationUser> _signInManager,
                            AppDbContext _context,
                            ITokenGenerator _tokenGenerator,
                            IConfiguration _configuration,
                            ILogger<IAuthService> _logger,
                            IUnitOfWork unitOfWork,
                            IMailService mailService) : IAuthService
    {
        public async Task<Result<LoginCommandResponse>> LoginAsync(string usernameOrEmail, string password)
        {
            var normalizedEmail = _userManager.NormalizeEmail(usernameOrEmail);
            var normalizedName = _userManager.NormalizeName(usernameOrEmail);

            var user = await _userManager.Users
                            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail
                                                   || u.NormalizedUserName == normalizedName);

            if (user is null)
                return ResultFactory.Failure<LoginCommandResponse>(ErrorType.BadInput, "Invalid credentials.");

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
            if (signInResult.Succeeded == false)
                return ResultFactory.Failure<LoginCommandResponse>(ErrorType.BadInput, "Invalid credentials.");

            if (user.EmailConfirmed == false)
                return ResultFactory.Failure<LoginCommandResponse>(ErrorType.BadInput, "Email not confirmed.");

            var roles = (await _userManager.GetRolesAsync(user))
                                .Select(x => Enum.Parse<Role>(x))
                                .ToArray();

            var expiryMinutes = int.Parse(_configuration[ConfigurationKeys.JwtExpiryMinutes]!);
            var expiryMinutesRefresh = int.Parse(_configuration[ConfigurationKeys.JwtRefreshExpiryMinutes]!);
            
            var accessTokenRes = _tokenGenerator.GenerateToken(user.Id, user.Email, roles, JwtTokenType.Jwt);
            if (accessTokenRes.IsSuccess == false)
                return ResultFactory.Failure<LoginCommandResponse>(accessTokenRes.ErrorType, accessTokenRes.Message);
            
            var refreshTokenRes = _tokenGenerator.GenerateToken(user.Id, user.Email, roles, JwtTokenType.Refresh);
            if (refreshTokenRes.IsSuccess == false)
                return ResultFactory.Failure<LoginCommandResponse>(refreshTokenRes.ErrorType, refreshTokenRes.Message);
            
            return ResultFactory.Success(new LoginCommandResponse
            (
                AccessToken: accessTokenRes.Data!,
                AccessTokenExpiry: DateTime.UtcNow.AddMinutes(expiryMinutes),
                RefreshToken: refreshTokenRes.Data!,
                RefreshTokenExpiry: DateTime.UtcNow.AddMinutes(expiryMinutesRefresh)
            ));
        }
        public async Task<Result<RegisterCommandResponse>> RegisterAsync(string username, string email, string password, Role[] roles)
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
                return ResultFactory.Failure<RegisterCommandResponse>(ErrorType.BadInput, "Registration failed: " + errors);
            }

            var roleRes = await _userManager.AddToRolesAsync(user, roles.Select(x => x.ToString()));
            if (!roleRes.Succeeded)
            {
                transaction.Rollback();
                _logger.LogError(string.Join(",", roleRes.Errors.Select(x => x.Description)));
                return ResultFactory.Failure<RegisterCommandResponse>(ErrorType.BadInput, "User registration failed duo to unknown error");
            }
            await transaction.CommitAsync();
            var link = await GenerateConfirmationLink(user);
            await mailService.SendConfirmMail("Documentify Email Confirmation", email, link, null);
            return ResultFactory.Success(new RegisterCommandResponse("Successful registration, please check your email address"));
        }

        public async Task<Result<RefreshTokenResponse>> RefreshTokenAsync(string oldRefreshToken, CancellationToken cancellationToken)
        {
            var validateResult = _tokenGenerator.ValidateRefreshToken(oldRefreshToken);
            if(validateResult.IsSuccess == false)
                return ResultFactory.Failure<RefreshTokenResponse>(validateResult.ErrorType, validateResult.Message);

            bool IsRevoked = await unitOfWork.Repository<RevokedRefreshToken, int>()
                                                    .ExistsAsync(x => x.Token == oldRefreshToken);

            if (IsRevoked)
                return ResultFactory.Failure<RefreshTokenResponse>(ErrorType.BadInput, "Refresh token has been revoked.");

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
                return ResultFactory.Failure<RefreshTokenResponse>(ErrorType.BadInput, "User not found");

            var roles = (await _userManager.GetRolesAsync(user))
                                .Select(Enum.Parse<Role>)
                                .ToArray();
            var accessTokenRes = _tokenGenerator.GenerateToken(user.Id, user.Email, roles, JwtTokenType.Jwt);
            if (accessTokenRes.IsSuccess == false)
                return ResultFactory.Failure<RefreshTokenResponse>(accessTokenRes.ErrorType, accessTokenRes.Message);
            
            var refreshTokenRes = _tokenGenerator.GenerateToken(user.Id, user.Email, roles, JwtTokenType.Refresh);
            if (refreshTokenRes.IsSuccess == false)
                return ResultFactory.Failure<RefreshTokenResponse>(refreshTokenRes.ErrorType, refreshTokenRes.Message);
            
            return ResultFactory.Success(new RefreshTokenResponse
            (
                AccessToken: accessTokenRes.Data!,
                AccessTokenExpiry: DateTime.UtcNow.AddMinutes(expiryMinutes),
                RefreshToken: refreshTokenRes.Data!,
                RefreshTokenExpiry: DateTime.UtcNow.AddMinutes(expiryMinutesRefresh)
            ));
        }

        public async Task<Result<ConfirmEmailResponse>> ConfirmEmailAsync(string token, string email, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return ResultFactory.Failure<ConfirmEmailResponse>(ErrorType.BadInput, "Invalid email confirmation request.");

            var res = await _userManager.ConfirmEmailAsync(user, token);
            if (!res.Succeeded)
            {
                var errors = string.Join(",", res.Errors.Select(e => e.Description));
                return ResultFactory.Failure<ConfirmEmailResponse>(ErrorType.BadInput, "Email confirmation failed: " + errors);
            }
            return ResultFactory.Success(new ConfirmEmailResponse(Message: "Email confirmed successfully."));
        }
        async Task<string> GenerateConfirmationLink(ApplicationUser user)
        {
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user!);
            var baseUrl = _configuration["ASPNETCORE_URLS"]?.Split(";").FirstOrDefault();
            var userMail = UrlEncoder.Default.Encode(user.Email!);
            token = UrlEncoder.Default.Encode(token);
            return $"{baseUrl}/Account/ConfirmEmail?token={token}&email={userMail}";
        }
    }
}
