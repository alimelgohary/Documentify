using Documentify.ApplicationCore.Common.Interfaces;
using MediatR;

namespace Documentify.ApplicationCore.Features.Auth.Login
{
    public class LoginCommandHandler(IAuthService authService) : IRequestHandler<LoginCommand, LoginCommandResponse>
    {
        public async Task<LoginCommandResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            return await authService.LoginAsync(request.UsernameOrEmail, request.Password);
        }
    }
}
