namespace Documentify.Domain.Entities
{
    public partial class UserRateOffice
    {
        public byte Rating { get; set; }
        public string? Comment { get; set; }
        public Guid OfficeId { get; set; }
        public Office Office { get; set; } = null!;
        public string RaterId { get; set; } = null!;
    }
    public partial class UserRateOffice
    {
        public static class ValidationConstants
        {
            public const int CommentMaxLength = 500;
            public const int CommentMinLength = 10;
        }
    }
}
