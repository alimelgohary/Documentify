using Documentify.ApplicationCore.Features.Categories.GetAll;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Documentify.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoriesController(IMediator _mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct) 
            => Ok(await _mediator.Send(new GetAllCategoriesQuery(), ct));
    }
}
