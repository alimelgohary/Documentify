using Documentify.ApplicationCore.Features;
using Documentify.ApplicationCore.Features.Categories;
using Documentify.ApplicationCore.Features.Categories.Add;
using Documentify.ApplicationCore.Features.Categories.GetAll;
using Documentify.ApplicationCore.Features.Categories.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Documentify.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoriesController(IMediator _mediator) : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<ActionResult<Result<CategoryDto>>> GetById(Guid id, CancellationToken ct) 
            => Ok(await _mediator.Send(new GetCategoryByIdQuery(id), ct)); 

        [HttpGet]
        public async Task<ActionResult<Result<GetAllCategoriesResponse>>> Index(CancellationToken ct)
            => Ok(await _mediator.Send(new GetAllCategoriesQuery(), ct));

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Result<AddCategoryResponse>>> Create([FromBody] AddCategoryCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            return CreatedAtAction(nameof(GetById), new { Id = result.Data!.categoryId }, result);
        }
    }
}
