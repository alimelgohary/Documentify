using Ardalis.Specification;
using Documentify.Domain.Entities;
namespace Documentify.ApplicationCore.Features.Categories.GetAll
{
    public class GetAllCategoriesSpec : Specification<Category>
    {
        public GetAllCategoriesSpec()
        {
            Query.AsNoTracking();
        }
    }
}
