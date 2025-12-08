using Documentify.ApplicationCore.Common.Interfaces;
using Documentify.Domain.Enums;
using MediatR;

namespace Documentify.ApplicationCore.Features.ExternalAuth
{
    public class ExternalLoginCommandHandler(IExternalAuthService externalAuthService) : IRequestHandler<ExternalLoginCommand, string>
    {
        public async Task<string> Handle(ExternalLoginCommand request, CancellationToken cancellationToken)
        {
            return await externalAuthService.LoginOrRegister(ExternalLoginProvider.Google);
        }
    }
}
