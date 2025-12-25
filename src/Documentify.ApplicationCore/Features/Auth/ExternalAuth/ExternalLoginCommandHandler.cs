using Documentify.ApplicationCore.Common.Interfaces;
using Documentify.ApplicationCore.Features.Auth.Login;
using Documentify.Domain.Enums;
using MediatR;

namespace Documentify.ApplicationCore.Features.Auth.ExternalAuth
{
    public class ExternalLoginCommandHandler(IExternalAuthService externalAuthService) : IRequestHandler<ExternalLoginCommand, Result<ExternalLoginCommandResponse>>
    {
        public async Task<Result<ExternalLoginCommandResponse>> Handle(ExternalLoginCommand request, CancellationToken cancellationToken)
        {
            return Result<ExternalLoginCommandResponse>
                    .Success(await externalAuthService.LoginOrRegister(ExternalLoginProvider.Google));
        }
    }
}
