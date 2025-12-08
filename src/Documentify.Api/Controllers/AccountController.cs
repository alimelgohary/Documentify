using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Documentify.ApplicationCore.Features.ExternalAuth;

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
        
    }
}