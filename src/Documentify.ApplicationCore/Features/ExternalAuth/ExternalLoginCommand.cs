using MediatR;
namespace Documentify.ApplicationCore.Features.ExternalAuth
{
    public record ExternalLoginCommand() : IRequest<string>;
}
