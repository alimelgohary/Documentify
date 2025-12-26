using MediatR;

namespace Documentify.ApplicationCore.Features.Categories.GetAll
{
    public record GetAllCategoriesQuery() : IRequest<Result<GetAllCategoriesResponse>>;
}
