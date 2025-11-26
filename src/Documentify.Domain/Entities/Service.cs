
using Documentify.Domain.Entities.Comment;
using Documentify.Domain.Entities.Common;
using Documentify.Domain.Entities.Report;

namespace Documentify.Domain.Entities
{
    public partial class Service : ServiceBase
    {
        public ICollection<ServiceSuggestion>? ServiceSuggestions { get; set; }
        public ICollection<ServiceComment>? ServiceComments { get; set; }
        public string? ApproverId { get; set; }
        public ICollection<ServiceReport>? Reports { get; set; }
    }
}
