using Documentify.ApplicationCore.Features;
using Documentify.ApplicationCore.Features.Auth.Login;
using Documentify.Domain.Enums;

namespace Documentify.ApplicationCore.Common.Interfaces
{
    public interface IExternalAuthService
    {
        Task<Result<ExternalLoginCommandResponse>> LoginOrRegister(ExternalLoginProvider externalLoginProvider);
    }
}
