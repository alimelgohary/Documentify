using Documentify.Domain.Entities.Common;

namespace Documentify.Domain.Entities
{
    public partial class AdminBanUser : EntityBase
    {
        public string? AdminId { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string? Reason { get; set; }
        public DateTime BannedAt { get; set; } = DateTime.UtcNow;
    }
    public partial class AdminBanUser
    {
        public static class ValidationConstants
        {
            public const int ReasonMaxLength = 200;
        }
    }
}
