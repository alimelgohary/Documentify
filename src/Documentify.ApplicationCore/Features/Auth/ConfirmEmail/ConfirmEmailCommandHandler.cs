using Documentify.ApplicationCore.Common.Interfaces;
using MediatR;

namespace Documentify.ApplicationCore.Features.Auth.ConfirmEmail
{
    public class ConfirmEmailCommandHandler(IAuthService _authService) : IRequestHandler<ConfirmEmailCommand, Result<ConfirmEmailResponse>>
    {
        public async Task<Result<ConfirmEmailResponse>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            return await _authService.ConfirmEmailAsync(request.Token, request.Email, cancellationToken);
        }
    }
}
