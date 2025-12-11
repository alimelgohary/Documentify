using Documentify.ApplicationCore.Common.Exceptions;
using Documentify.ApplicationCore.Common.Interfaces;
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
        public string GenerateToken(string userId, string userEmail, JwtTokenType type)
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
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public void ValidateRefreshToken(string oldRefreshToken)
        {
            var secret = _configuration[ConfigurationKeys.JwtRefreshSecret]!;
            string issuer = _configuration[ConfigurationKeys.JwtIssuer]!;
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            try
            {
                _logger.LogError(tokenHandler.ReadJwtToken(oldRefreshToken).Issuer);
                tokenHandler.ValidateToken(oldRefreshToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = false,
                    ValidateLifetime = true
                }, out SecurityToken token);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Invalid refresh token attempted" + ex.Message);
                throw new BadRequestException("Validating token error");
            }
        }

        public string GenerateToken(string expiredToken, JwtTokenType type)
        {
            var token = new JwtSecurityTokenHandler().ReadJwtToken(expiredToken);
            var email = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value
                    ?? throw new BadRequestException("Invalid refresh token, no email");

            var id = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                    ?? throw new BadRequestException("Invalid refresh token, no sub");

            return GenerateToken(id, email, type);
        }
    }
}
