using MediatR;

namespace Documentify.ApplicationCore.Features.Auth.Register
{
    public record RegisterCommand(string Username, string Email, string Password) : IRequest<RegisterCommandResponse>;
}
