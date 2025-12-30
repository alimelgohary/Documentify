using Documentify.ApplicationCore.Features;
using Documentify.ApplicationCore.Features.Auth.ConfirmEmail;
using Documentify.ApplicationCore.Features.Auth.Login;
using Documentify.ApplicationCore.Features.Auth.RefreshToken;
using Documentify.ApplicationCore.Features.Auth.Register;
using Documentify.Domain.Enums;

namespace Documentify.ApplicationCore.Common.Interfaces
{
    public interface IAuthService
    {
        Task<Result<LoginCommandResponse>> LoginAsync(string usernameOrEmail, string password);
        Task<Result<RefreshTokenResponse>> RefreshTokenAsync(string oldRefreshToken, CancellationToken cancellationToken);
        Task<Result<RegisterCommandResponse>> RegisterAsync(string username, string email, string password, Role[] roles);
        Task<Result<ConfirmEmailResponse>> ConfirmEmailAsync(string token, string email, CancellationToken ct = default);
    }
}
