using Documentify.ApplicationCore.Common.Interfaces;
using MediatR;

namespace Documentify.ApplicationCore.Features.Auth.RefreshToken
{
    public class RefreshTokenHandler(IAuthService _authService) : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>
    {
        public async Task<Result<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            return await _authService.RefreshTokenAsync(request.OldRefreshToken, cancellationToken);
        }
    }
}
