namespace Documentify.ApplicationCore.Features.Categories.GetAll
{
    public record GetAllCategoriesResponse(IEnumerable<CategoryDto> Items, int Count);
}
