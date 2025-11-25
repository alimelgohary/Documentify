
using Documentify.Domain.Entities.Common;
using System.Text;

namespace Documentify.Domain.Entities
{
    public partial class Office : OfficeBase
    {
        public string? ApproverId { get; set; }
        public ICollection<OfficeStatus>? OfficeStatuses { get; set; }
        public ICollection<UserRateOffice>? UserRateOffices { get; set; }
        public ICollection<OfficeSuggestion>? OfficeSuggestions { get; set; }
    }
}
