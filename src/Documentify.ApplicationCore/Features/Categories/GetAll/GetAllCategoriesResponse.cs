namespace Documentify.ApplicationCore.Features.Categories.GetAll
{
    public record GetAllCategoriesResponse(List<CategoryDto> items, int count);
}
