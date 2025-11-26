using Documentify.Domain.Entities.Comment;
using Documentify.Domain.Entities.Common;

namespace Documentify.Domain.Entities
{
    public partial class ServiceSuggestion : ServiceBase
    {
        public SuggestionType SuggestionType { get; set; }
        public string Change { get; set; } = null!;
        public Guid? ServiceId { get; set; }
        public Service? Service { get; set; }
        public ICollection<ServiceSuggestionComment>? ServiceSuggestionComments { get; set; }
    }
    public partial class ServiceSuggestion : ServiceBase
    {
        public static class ValidationConstants
        {
            public const int ChangeMaxLength = 300;
        }
    }
}
