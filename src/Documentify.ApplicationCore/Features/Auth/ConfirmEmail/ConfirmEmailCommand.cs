using MediatR;

namespace Documentify.ApplicationCore.Features.Auth.ConfirmEmail
{
    public record ConfirmEmailCommand(string Token) : IRequest<ConfirmEmailResponse>;
}
