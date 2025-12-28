using MediatR;

namespace Documentify.ApplicationCore.Features.Auth.ConfirmEmail
{
    public record ConfirmEmailCommand(string Token, string Email) : IRequest<Result<ConfirmEmailResponse>>;
}
