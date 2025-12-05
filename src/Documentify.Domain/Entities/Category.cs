
using Documentify.Domain.Entities.Common;

namespace Documentify.Domain.Entities
{
    public partial class Category : EntityBase
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<ServiceBase>? Services { get; set; }
    }
    public partial class Category
    {
        public static class ValidationConstants
        {
            public const int NameMaxLength = 100;
            public const int NameMinLength = 3;
        }
    }
}
