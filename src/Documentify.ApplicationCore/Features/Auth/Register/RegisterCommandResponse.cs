namespace Documentify.ApplicationCore.Features.Auth.Register
{
    public record RegisterCommandResponse(string token, DateTime expirationDate);
}
