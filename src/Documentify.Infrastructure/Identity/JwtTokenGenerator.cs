using Documentify.ApplicationCore.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Documentify.Infrastructure.InfrastructureServiceExtensions;

namespace Documentify.Infrastructure.Identity
{
    public class JwtTokenGenerator(IConfiguration _configuration) : ITokenGenerator
    {
        public string GenerateToken(string userId, string userEmail)
        {
            int expiryMinutes = int.Parse(_configuration[ConfigurationKeys.JwtExpiryMinutes]!);
            string secret = _configuration[ConfigurationKeys.JwtSecret]!;
            string issuer = _configuration[ConfigurationKeys.JwtIssuer]!;
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, userEmail)
            };
            
            var key = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
