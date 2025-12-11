using Documentify.ApplicationCore.Features.Auth.Login;
using Documentify.Domain.Enums;

namespace Documentify.ApplicationCore.Common.Interfaces
{
    public interface IExternalAuthService
    {
        Task<ExternalLoginCommandResponse> LoginOrRegister(ExternalLoginProvider externalLoginProvider);
    }
}
