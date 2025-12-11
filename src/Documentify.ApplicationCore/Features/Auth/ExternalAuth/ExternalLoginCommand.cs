using MediatR;
namespace Documentify.ApplicationCore.Features.Auth.ExternalAuth
{
    public record ExternalLoginCommand() : IRequest<string>;
}
