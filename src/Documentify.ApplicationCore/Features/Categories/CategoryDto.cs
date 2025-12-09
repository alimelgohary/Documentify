using Documentify.Domain.Entities;

namespace Documentify.ApplicationCore.Features.Categories
{
    public record CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        
        public CategoryDto(Guid Id, string Name)
        {
            this.Id = Id;
            this.Name = Name;
        }
        public CategoryDto(): this(Guid.Empty, string.Empty) { }
        public CategoryDto(Category category) : this(category.Id, category.Name) { }
    }
}