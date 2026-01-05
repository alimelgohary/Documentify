using Microsoft.AspNetCore.Mvc;

namespace Documentify.Api.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class SwaggerController : ControllerBase
    {
        [Route("/")]
        public IActionResult Index()
        {
            return Redirect("~/swagger/index.html");
        }
    }
}
