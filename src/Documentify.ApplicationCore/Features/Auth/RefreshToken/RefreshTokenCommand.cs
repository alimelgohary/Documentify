using MediatR;

namespace Documentify.ApplicationCore.Features.Auth.RefreshToken
{
    public record RefreshTokenCommand(string OldRefreshToken) : IRequest<Result<RefreshTokenResponse>>; 
}
