using Documentify.ApplicationCore.Repository;
using Documentify.Domain.Entities;
using MediatR;
namespace Documentify.ApplicationCore.Features.Categories.Add
{
    public class AddCategoryCommandHandler(IUnitOfWork _unitOfWork) : IRequestHandler<AddCategoryCommand, Result<AddCategoryResponse>>
    {
        public async Task<Result<AddCategoryResponse>> Handle(AddCategoryCommand request, CancellationToken ct)
        {
            // add pipeline to logging / handle not found exceptions
            var category = new Category() { Name = request.categoryName };
            await _unitOfWork.Repository<Category, Guid>().AddAsync(category, ct);
            await _unitOfWork.CompleteAsync(ct);
            return Result<AddCategoryResponse>.Success(
                new AddCategoryResponse(categoryId: category.Id));
        }
    }
}
