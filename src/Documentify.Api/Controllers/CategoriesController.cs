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
        public async Task<ActionResult<CategoryDto>> GetById(Guid id, CancellationToken ct) 
            => Ok(await _mediator.Send(new GetCategoryByIdQuery(id), ct)); 

        [HttpGet]
        public async Task<ActionResult<GetAllCategoriesResponse>> Index(CancellationToken ct)
            => Ok(await _mediator.Send(new GetAllCategoriesQuery(), ct));

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<AddCategoryResponse>> Create([FromBody] AddCategoryCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            return CreatedAtAction(nameof(GetById), new { Id = result.categoryId }, result);
        }
    }
}
