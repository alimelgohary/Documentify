using Documentify.ApplicationCore.Features.Categories.Add;
using Documentify.ApplicationCore.Features.Categories.GetAll;
using FluentValidation;
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

        [HttpPost]
        public async Task<ActionResult<AddCategoryResponse>> Create([FromBody] AddCategoryCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            return CreatedAtAction("Index", new { Id = result.categoryId }, result);
        }
    }
}
