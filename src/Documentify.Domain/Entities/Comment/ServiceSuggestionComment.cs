using Documentify.Domain.Entities.Common;

namespace Documentify.Domain.Entities.Comment
{
    public class ServiceSuggestionComment : CommentBase
    {
        public Guid ServiceSuggestionId { get; set; }
        public ServiceSuggestion ServiceSuggestion { get; set; } = null!;
    }
}
