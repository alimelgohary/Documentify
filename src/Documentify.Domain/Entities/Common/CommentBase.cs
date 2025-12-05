namespace Documentify.Domain.Entities.Common
{
    public abstract partial class CommentBase : EntityBase
    {
        public Guid Id { get; set; }
        public string CommenterId { get; set; } = null!;
        public DateTime CommentedAt { get; set; }
        public string Comment { get; set; } = null!;
    }
    public abstract partial class CommentBase
    {
        public static class ValidationConstants
        {
            public const int CommentMaxLength = 200;
        }
    }
}
