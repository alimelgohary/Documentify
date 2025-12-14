using Documentify.Infrastructure.Data;
using Documentify.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Documentify.IntegrationTests
{
    public class NoAuthNoDbWebApplicationFactory<T> : NoDbWebApplicationFactory<T> where T : class
    {
        override protected void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.ConfigureServices(services =>
            {
                // Remove existing authentication handlers if any
                var authDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IAuthenticationSchemeProvider));
                if (authDescriptor != null)
                {
                    services.Remove(authDescriptor);
                }
                // Add a no-authentication scheme for testing
                services.AddAuthentication("NoAuth")
                    .AddScheme<AuthenticationSchemeOptions, NoAuthAuthenticationHandler>(
                        "NoAuth", options => { });
                // Configure the app to use the no-auth scheme by default
                services.PostConfigure<AuthenticationOptions>(options =>
                {
                    options.DefaultAuthenticateScheme = "NoAuth";
                    options.DefaultChallengeScheme = "NoAuth";
                });
            });
        }
    }
    public class NoAuthAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            UserManager<ApplicationUser> _userManager) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var user = new ApplicationUser
            {
                UserName = "TestUser0",
                Email = "user0@gmail.com"
            };
            await _userManager.CreateAsync(user);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, "Admin")
            };
            var identity = new ClaimsIdentity(claims, "NoAuth");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "NoAuth");
            return AuthenticateResult.Success(ticket);
        }
    }
}
