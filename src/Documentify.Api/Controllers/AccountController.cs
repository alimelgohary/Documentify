using Documentify.Api;
using Documentify.ApplicationCore.Features;
using Documentify.ApplicationCore.Features.Auth.ConfirmEmail;
using Documentify.ApplicationCore.Features.Auth.ExternalAuth;
using Documentify.ApplicationCore.Features.Auth.Login;
using Documentify.ApplicationCore.Features.Auth.RefreshToken;
using Documentify.ApplicationCore.Features.Auth.Register;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<Result<ExternalLoginCommandResponse>>> GoogleResponse() 
            => (await _sender.Send(new ExternalLoginCommand())).ToActionResult();

        [HttpPost("Register")]
        public async Task<ActionResult<Result<RegisterCommandResponse>>> Register(RegisterCommand command) 
            => (await _sender.Send(command)).ToActionResult();

        [HttpPost("Login")]
        public async Task<ActionResult<Result<LoginCommandResponse>>> Login(LoginCommand command) 
            => (await _sender.Send(command)).ToActionResult();

        [HttpPost("RefreshToken")]
        public async Task<ActionResult<Result<RefreshTokenResponse>>> RefreshToken(RefreshTokenCommand refreshTokenCommand)
            => (await _sender.Send(refreshTokenCommand)).ToActionResult();


        [HttpGet(nameof(ConfirmEmail))]
        public async Task<ActionResult<Result<ConfirmEmailResponse>>> ConfirmEmail([FromQuery] ConfirmEmailCommand confirmEmailCommand)
            => (await _sender.Send(confirmEmailCommand)).ToActionResult();
    }
}