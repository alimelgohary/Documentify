using Documentify.ApplicationCore.Common.Interfaces;
using Documentify.ApplicationCore.Features;
using Documentify.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Documentify.Infrastructure.InfrastructureServiceExtensions;

namespace Documentify.Infrastructure.Identity
{
    public class JwtTokenGenerator(IConfiguration _configuration, ILogger<JwtTokenGenerator> _logger) : ITokenGenerator
    {
        public Result<string> GenerateToken(string userId, string userEmail, Role[] roles, JwtTokenType type)
        {
            int expiryMinutes = 0;
            string secret = "";
            if (type == JwtTokenType.Jwt)
            {
                expiryMinutes = int.Parse(_configuration[ConfigurationKeys.JwtExpiryMinutes]!);
                secret = _configuration[ConfigurationKeys.JwtSecret]!;
            }
            else if (type == JwtTokenType.Refresh)
            {
                expiryMinutes = int.Parse(_configuration[ConfigurationKeys.JwtRefreshExpiryMinutes]!);
                secret = _configuration[ConfigurationKeys.JwtRefreshSecret]!;
            }

            string issuer = _configuration[ConfigurationKeys.JwtIssuer]!;
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, userEmail)
            }.Concat(roles.Select(x => new Claim(ClaimTypes.Role, x.ToString())));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: creds
            );

            return ResultFactory.Success<string>(new JwtSecurityTokenHandler().WriteToken(token));
        }

        public Result ValidateRefreshToken(string oldRefreshToken)
        {
            var secret = _configuration[ConfigurationKeys.JwtRefreshSecret]!;
            string issuer = _configuration[ConfigurationKeys.JwtIssuer]!;
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            try
            {
                tokenHandler.ValidateToken(oldRefreshToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken token);
                return ResultFactory.Success();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Invalid refresh token attempted" + ex.Message);
                return ResultFactory.Failure(ErrorType.BadInput, "Validating token error");
            }
        }

        public Result<string> GenerateToken(string expiredToken, JwtTokenType type)
        {
            var token = new JwtSecurityTokenHandler().ReadJwtToken(expiredToken);
            var email = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (email is null) return ResultFactory.Failure<string>(ErrorType.BadInput, "Invalid refresh token, no email");

            var id = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if(id is null) return ResultFactory.Failure<string>(ErrorType.BadInput, "Invalid token, no sub");

            var rolesClaims = token.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();

            List<Role> roles = new List<Role>(rolesClaims.Count);
            foreach (var role in rolesClaims)
            {
                if (Enum.TryParse(role.Value, out Role r))
                    roles.Add(r);
                else
                    return ResultFactory.Failure<string>(ErrorType.BadInput, "Invalid token, invalid role");
            }
            return GenerateToken(id, email, roles.ToArray(), type);
        }
    }
}
