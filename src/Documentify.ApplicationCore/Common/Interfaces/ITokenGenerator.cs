using Documentify.ApplicationCore.Features;
using Documentify.Domain.Enums;

namespace Documentify.ApplicationCore.Common.Interfaces
{
    public interface ITokenGenerator
    {
        Result<string> GenerateToken(string userId, string userEmail, Role[] roles, JwtTokenType type);
        Result<string> GenerateToken(string expiredToken, JwtTokenType type);
        Result ValidateRefreshToken(string oldRefreshToken);
    }
    public enum JwtTokenType
    {
        Jwt,
        Refresh
    }
}
