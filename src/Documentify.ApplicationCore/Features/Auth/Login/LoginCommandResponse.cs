namespace Documentify.ApplicationCore.Features.Auth.Login
{
    public record LoginCommandResponse(string AccessToken, DateTime AccessTokenExpiry, string RefreshToken, DateTime RefreshTokenExpiry);
}