using Documentify.Domain.Enums;

namespace Documentify.ApplicationCore.Common.Interfaces
{
    public interface ITokenGenerator
    {
        string GenerateToken(string userId, string userEmail, Role[] roles, JwtTokenType type);
        string GenerateToken(string expiredToken, JwtTokenType type);
        void ValidateRefreshToken(string oldRefreshToken);
    }
    public enum JwtTokenType
    {
        Jwt,
        Refresh
    }
}
