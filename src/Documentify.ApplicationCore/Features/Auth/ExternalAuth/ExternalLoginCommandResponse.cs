namespace Documentify.ApplicationCore.Features.Auth.Login
{
    public record ExternalLoginCommandResponse(string AccessToken, DateTime AccessTokenExpiry, string RefreshToken, DateTime RefreshTokenExpiry);
}