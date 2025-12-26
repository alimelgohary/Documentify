using Documentify.ApplicationCore.Common.Interfaces;
using MediatR;

namespace Documentify.ApplicationCore.Features.Auth.Login
{
    public class LoginCommandHandler(IAuthService authService) : IRequestHandler<LoginCommand, Result<LoginCommandResponse>>
    {
        public async Task<Result<LoginCommandResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            return Result<LoginCommandResponse>.Success(
                await authService.LoginAsync(request.UsernameOrEmail, request.Password));
        }
    }
}
