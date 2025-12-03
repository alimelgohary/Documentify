using Documentify.ApplicationCore.Repository;
using Documentify.Domain.Entities;
using MediatR;
namespace Documentify.ApplicationCore.Features.Categories.Add
{
    public class AddCategoryCommandHandler(IUnitOfWork<Category, Guid> _unitOfWork) : IRequestHandler<AddCategoryCommand, AddCategoryResponse>
    {
        public async Task<AddCategoryResponse> Handle(AddCategoryCommand request, CancellationToken ct)
        {
            // add pipeline to logging / handle not found exceptions
            var category = new Category() { Name = request.categoryName };
            await _unitOfWork.Repository.AddAsync(category, ct);
            await _unitOfWork.CompleteAsync(ct);
            return new AddCategoryResponse(categoryId: category.Id);
        }
    }
}
