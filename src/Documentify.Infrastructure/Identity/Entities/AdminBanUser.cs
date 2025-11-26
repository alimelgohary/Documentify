using Documentify.Infrastructure.Identity.Entities;

namespace Documentify.Infrastructure.Identity
{
    public partial class AdminBanUser
    {
        public string? AdminId { get; set; } = null!;
        public ApplicationUser? Admin { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
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
