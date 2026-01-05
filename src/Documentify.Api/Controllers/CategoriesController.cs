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
        /// <summary>
        ///     Retrieve a category by its unique identifier.
        /// </summary>
        /// <param name="id">The unique category identifier</param>
        /// <response code="404">If the item is not found.</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<Result<CategoryDto>>> GetById(Guid id, CancellationToken ct) 
            => (await _mediator.Send(new GetCategoryByIdQuery(id), ct)).ToActionResult();

        /// <summary>
        ///     Gets all categories.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<Result<GetAllCategoriesResponse>>> Index(CancellationToken ct)
            => (await _mediator.Send(new GetAllCategoriesQuery(), ct)).ToActionResult();

        /// <summary>
        ///     Adds a new category.
        /// </summary>
        /// <param name="command">AddCategoryCommand</param>
        /// <response code="400">Validation error</response>
        /// <response code="201">Created response with the new category id and location header</response>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Result<AddCategoryResponse>>> Create([FromBody] AddCategoryCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            if(!result.IsSuccess)
                return result.ToActionResult();
            return CreatedAtAction(nameof(GetById), new { Id = result.Data!.categoryId }, result);
        }
    }
}
