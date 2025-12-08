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
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, userEmail)
            };
            
            var key = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(_configuration[ConfigurationKeys.JwtSecret]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration[ConfigurationKeys.JwtIssuer],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
