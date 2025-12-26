using Documentify.ApplicationCore.Repository;
using Documentify.Domain.Entities;
using MediatR;

namespace Documentify.ApplicationCore.Features.Categories.GetAll
{
    public class GetAllCategoriesQueryHandler(IUnitOfWork _unitOfWork) : IRequestHandler<GetAllCategoriesQuery, Result<GetAllCategoriesResponse>>
    {
        public async Task<Result<GetAllCategoriesResponse>> Handle(GetAllCategoriesQuery request, CancellationToken ct)
        {
            var categories = await _unitOfWork
                .Repository<Category, Guid>()
                .ListAsync(new GetAllCategoriesSpec(), ct);
            var items = categories.Select(x => new CategoryDto(x.Id, x.Name)).ToList();
            return Result < GetAllCategoriesResponse >.Success(
                new GetAllCategoriesResponse(items, items.Count));
        }
    }
}
