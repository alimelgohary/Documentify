
using Documentify.Domain.Entities.Common;

namespace Documentify.Domain.Entities
{
    public partial class Service : ServiceBase
    {
        public ICollection<ServiceSuggestion>? ServiceSuggestions { get; set; }
        public string? ApproverId { get; set; }
    }
}
