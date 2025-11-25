using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Documentify.Domain.Entities
{
    public partial class OfficeStatus
    {
        public Guid Id { get; set; }
        public Guid OfficeId { get; set; }
        public Office Office { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Description { get; set; } = null!;
    }
    public partial class OfficeStatus
    {
        public static class ValidationConstants
        {
            public const int DescriptionMaxLength = 100;
            public const int DescriptionMinLength = 3;
        }
    }
}
