using Documentify.Domain.Enums;

namespace Documentify.ApplicationCore.Common.Interfaces
{
    public interface IExternalAuthService
    {
        Task<string> LoginOrRegister(ExternalLoginProvider externalLoginProvider);
    }
}
