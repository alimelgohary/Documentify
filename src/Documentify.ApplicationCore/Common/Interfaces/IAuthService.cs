using Documentify.ApplicationCore.Features.Auth.Login;
using Documentify.ApplicationCore.Features.Auth.RefreshToken;
using Documentify.ApplicationCore.Features.Auth.Register;
using Documentify.Domain.Enums;

namespace Documentify.ApplicationCore.Common.Interfaces
{
    public interface IAuthService
    {
        Task<LoginCommandResponse> LoginAsync(string usernameOrEmail, string password);
        Task<RefreshTokenResponse> RefreshTokenAsync(string oldRefreshToken, CancellationToken cancellationToken);
        Task<RegisterCommandResponse> RegisterAsync(string username, string email, string password, Role[] roles);
    }
}
