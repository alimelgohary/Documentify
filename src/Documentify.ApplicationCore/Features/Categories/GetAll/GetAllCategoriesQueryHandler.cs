using MediatR;

namespace Documentify.ApplicationCore.Features.Categories.GetAll
{
    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, GetAllCategoriesResponse>
    {
        public Task<GetAllCategoriesResponse> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new GetAllCategoriesResponse([new(Guid.NewGuid(), "Cat1")], 1));
        }
    }
}
