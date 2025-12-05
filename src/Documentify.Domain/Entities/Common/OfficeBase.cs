using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Documentify.Domain.Entities.Common
{
    public partial class OfficeBase : EntityBase
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        public Workdays? Workdays { get; set; }
        public TimeOnly? OpenFrom { get; set; }
        public TimeOnly? OpenTo { get; set; }
        public TimeOnly? EveningOpenFrom { get; set; }
        public TimeOnly? EveningOpenTo { get; set; }
        public double? LocationLat { get; set; }
        public double? LocationLng { get; set; }
        public string? LocationText { get; set; }
        public bool Is24_7 { get; set; }
        public string[]? Phones { get; set; }
        public string? WriterId { get; set; }
        public ICollection<ServiceBase>? Services { get; set; }
    }
    public partial class OfficeBase
    {
        public static class ValidationConstants
        {
            public const int NameMaxLength = 200;
            public const int NameMinLength = 5;
            public const int PhonesMaxLength = 15;
            public const int LocationTextMaxLength = 200;
        }
    }
}
