namespace Documentify.ApplicationCore.Common.Interfaces
{
    public interface ITokenGenerator
    {
        string GenerateToken(string userId, string userEmail, JwtTokenType type);
        string GenerateToken(string expiredToken, JwtTokenType type);
        void ValidateRefreshToken(string oldRefreshToken);
    }
    public enum JwtTokenType
    {
        Jwt,
        Refresh
    }
}
