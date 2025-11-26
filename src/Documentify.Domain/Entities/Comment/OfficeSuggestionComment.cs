using Documentify.Domain.Entities.Common;

namespace Documentify.Domain.Entities.Comment
{
    public class OfficeSuggestionComment : CommentBase
    {
        public Guid OfficeSuggestionId { get; set; }
        public OfficeSuggestion OfficeSuggestion { get; set; } = null!;
    }
}
