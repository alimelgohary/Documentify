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
        /// <summary>
        ///     Initiates the Google authentication process by redirecting the user to the Google login page.
        /// </summary>
        /// <remarks>
        ///     Upon successful authentication, the user is redirected to the GoogleResponse action.
        /// </remarks>
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

        /// <summary>
        ///     Google redirects user to this action
        /// </summary>
        /// <response code="400">Cannot login or register user</response>
        [AllowAnonymous]
        [HttpGet("GoogleResponse")]
        public async Task<ActionResult<Result<ExternalLoginCommandResponse>>> GoogleResponse()
            => (await _sender.Send(new ExternalLoginCommand())).ToActionResult();

        /// <summary>
        ///     Registers a new user
        /// </summary>
        /// <remarks>
        ///     This sends a confirmation email to the user with a link to confirm their email address.
        /// </remarks>
        /// <param name="command">RegisterCommand</param>
        /// <response code="400">Validation or registration error with errors dictionary</response>
        [HttpPost("Register")]
        public async Task<ActionResult<Result<RegisterCommandResponse>>> Register(RegisterCommand command)
            => (await _sender.Send(command)).ToActionResult();

        /// <summary>
        ///     Login an existing user
        /// </summary>
        /// <param name="command"></param>
        /// <response code="400">Invalid credentials or email not confirmed</response>
        [HttpPost("Login")]
        public async Task<ActionResult<Result<LoginCommandResponse>>> Login(LoginCommand command)
            => (await _sender.Send(command)).ToActionResult();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="refreshTokenCommand"></param>
        /// <response code="400">Bad token or user not found</response>
        [HttpPost("RefreshToken")]
        public async Task<ActionResult<Result<RefreshTokenResponse>>> RefreshToken(RefreshTokenCommand refreshTokenCommand)
            => (await _sender.Send(refreshTokenCommand)).ToActionResult();

        /// <summary>
        ///     Confirms a user's email address using the provided token. 
        ///     This endpoint is typically accessed via a link sent to the user's email.
        /// </summary>
        /// <param name="confirmEmailCommand"></param>
        /// <response code="400">Invalid email or token</response>
        [HttpGet(nameof(ConfirmEmail))]
        public async Task<ActionResult<Result<ConfirmEmailResponse>>> ConfirmEmail([FromQuery] ConfirmEmailCommand confirmEmailCommand)
            => (await _sender.Send(confirmEmailCommand)).ToActionResult();
    }
}