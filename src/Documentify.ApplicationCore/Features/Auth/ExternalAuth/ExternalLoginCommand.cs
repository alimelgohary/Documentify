using Documentify.ApplicationCore.Features.Auth.Login;
using MediatR;
namespace Documentify.ApplicationCore.Features.Auth.ExternalAuth
{
    public record ExternalLoginCommand() : IRequest<Result<ExternalLoginCommandResponse>>;
}
