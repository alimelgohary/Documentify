using Documentify.ApplicationCore.Features.Auth.Login;
using Documentify.ApplicationCore.Features.Auth.Register;

namespace Documentify.ApplicationCore.Common.Interfaces
{
    public interface IAuthService
    {
        Task<LoginCommandResponse> LoginAsync(string usernameOrEmail, string password);
        Task<RegisterCommandResponse> RegisterAsync(string username, string email, string password);
    }
}
