namespace Documentify.ApplicationCore.Features.Auth.Login
{
    public record LoginCommandResponse(string Token, DateTime Expiration);
}