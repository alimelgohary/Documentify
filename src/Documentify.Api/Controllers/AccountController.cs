using Documentify.ApplicationCore.Features;
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
            => Ok(await _sender.Send(new ExternalLoginCommand()));

        [HttpPost("Register")]
        public async Task<ActionResult<Result<RegisterCommandResponse>>> Register(RegisterCommand command) 
            => Ok(await _sender.Send(command));

        [HttpPost("Login")]
        public async Task<ActionResult<Result<LoginCommandResponse>>> Login(LoginCommand command) 
            => Ok(await _sender.Send(command));

        [HttpPost("RefreshToken")]
        public async Task<ActionResult<Result<RefreshTokenResponse>>> RefreshToken(RefreshTokenCommand refreshTokenCommand)
            => Ok(await _sender.Send(refreshTokenCommand));


        //[HttpPost(nameof(ConfirmEmail))]
        //public async Task<ActionResult<ConfirmEmailResponse>> ConfirmEmail([FromQuery] ConfirmEmailCommand confirmEmailCommand)
        //    => Ok(await _sender.Send(confirmEmailCommand));
    }
}