using Documentify.ApplicationCore.Common.Interfaces;
using Documentify.Domain.Enums;
using MediatR;

namespace Documentify.ApplicationCore.Features.Auth.Register
{
    public class RegisterCommandHandler(IAuthService _authService) : IRequestHandler<RegisterCommand, Result<RegisterCommandResponse>>
    {
        public async Task<Result<RegisterCommandResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            return await _authService.RegisterAsync(request.Username, request.Email, request.Password, [Role.User]);
        }
    }
}
