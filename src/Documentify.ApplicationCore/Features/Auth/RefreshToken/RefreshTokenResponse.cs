namespace Documentify.ApplicationCore.Features.Auth.RefreshToken
{
    public record RefreshTokenResponse(string AccessToken, DateTime AccessTokenExpiry, string RefreshToken, DateTime RefreshTokenExpiry);
}