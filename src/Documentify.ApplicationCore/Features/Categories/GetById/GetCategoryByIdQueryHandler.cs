using Documentify.ApplicationCore.Repository;
using Documentify.Domain.Entities;
using MediatR;

namespace Documentify.ApplicationCore.Features.Categories.GetById
{
    public class GetCategoryByIdQueryHandler(IUnitOfWork _unitOfWork) : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDto>>
    {
        public async Task<Result<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken ct)
        {
            Category? category = await _unitOfWork.Repository<Category, Guid>().GetByIdAsync(request.id, ct);
            if (category is null)
                return ResultFactory.Failure<CategoryDto>(ErrorType.NotFound, $"Category with id {request.id} not found.");

            return ResultFactory.Success(new CategoryDto(category));
        }
    }
}
