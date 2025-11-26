using Documentify.Domain.Entities.Comment;
using Documentify.Domain.Entities.Common;

namespace Documentify.Domain.Entities
{
    public partial class OfficeSuggestion : OfficeBase
    {
        public SuggestionType SuggestionType { get; set; }
        public string Change { get; set; } = null!;
        public Guid? OfficeId { get; set; }
        public Office? Office { get; set; }
        public ICollection<OfficeSuggestionComment>? OfficeSuggestionComments { get; set; }
    }
    public partial class OfficeSuggestion
    {
        public static class ValidationConstants
        {
            public const int ChangeMaxLength = 300;
        }
    }
}
