using MediatR;

namespace Documentify.ApplicationCore.Features.Auth.Login
{
    public record LoginCommand(string UsernameOrEmail, string Password) : IRequest<LoginCommandResponse>;
}
