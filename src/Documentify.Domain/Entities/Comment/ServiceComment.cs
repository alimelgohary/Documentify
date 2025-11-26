using Documentify.Domain.Entities.Common;

namespace Documentify.Domain.Entities.Comment
{
    public class ServiceComment : CommentBase
    {
        public Guid ServiceId { get; set; }
        public Service Service { get; set; } = null!;
    }
}
