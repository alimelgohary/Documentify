using Documentify.ApplicationCore.Features.Auth.Login;
using Documentify.ApplicationCore.Features.Auth.Register;
using Documentify.ApplicationCore.Features.Auth.RefreshToken;
using Documentify.ApplicationCore.Features.Auth.ExternalAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace Documentify.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController(ISender _sender) : ControllerBase
    {
        [HttpGet("Google/Login")]
        [AllowAnonymous]
        public IActionResult Login()
        {
            var redirectUri = Url.Action("GoogleResponse");
            var properties = new AuthenticationProperties(
                new Dictionary<string, string?>() {
                    //{".redirect", redirectUri },
                    {"LoginProvider", "Google" }
            });
            properties.RedirectUri = redirectUri;
            return Challenge(properties, "Google");
        }

        [AllowAnonymous]
        [HttpGet("GoogleResponse")]
        public async Task<IActionResult> GoogleResponse() 
            => Ok(await _sender.Send(new ExternalLoginCommand()));

        [HttpPost("Register")]
        public async Task<ActionResult<RegisterCommandResponse>> Register(RegisterCommand command) 
            => Ok(await _sender.Send(command));

        [HttpPost("Login")]
        public async Task<ActionResult<LoginCommandResponse>> Login(LoginCommand command) 
            => Ok(await _sender.Send(command));

        [HttpPost("RefreshToken")]
        public async Task<ActionResult<RefreshTokenResponse>> RefreshToken(RefreshTokenCommand refreshTokenCommand)
            => Ok(await _sender.Send(refreshTokenCommand));

    }
}