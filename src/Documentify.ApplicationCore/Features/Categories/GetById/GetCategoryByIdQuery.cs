using MediatR;
namespace Documentify.ApplicationCore.Features.Categories.GetById
{
    public record GetCategoryByIdQuery(Guid id) : IRequest<Result<CategoryDto>>;
}
