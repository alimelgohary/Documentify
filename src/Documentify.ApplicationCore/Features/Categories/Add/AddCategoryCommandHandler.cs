using MediatR;
namespace Documentify.ApplicationCore.Features.Categories.Add
{
    public class AddCategoryCommandHandler : IRequestHandler<AddCategoryCommand, AddCategoryResponse>
    {
        public async Task<AddCategoryResponse> Handle(AddCategoryCommand request, CancellationToken cancellationToken)
        {
            var x = request.categoryName;
            // add pipeline to validate object/ logging / handle not found
            return new(Guid.NewGuid());
        }
    }
}
