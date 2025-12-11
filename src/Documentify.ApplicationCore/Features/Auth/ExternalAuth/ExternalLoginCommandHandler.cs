using Documentify.ApplicationCore.Common.Interfaces;
using Documentify.ApplicationCore.Features.Auth.Login;
using Documentify.Domain.Enums;
using MediatR;

namespace Documentify.ApplicationCore.Features.Auth.ExternalAuth
{
    public class ExternalLoginCommandHandler(IExternalAuthService externalAuthService) : IRequestHandler<ExternalLoginCommand, ExternalLoginCommandResponse>
    {
        public async Task<ExternalLoginCommandResponse> Handle(ExternalLoginCommand request, CancellationToken cancellationToken)
        {
            return await externalAuthService.LoginOrRegister(ExternalLoginProvider.Google);
        }
    }
}
