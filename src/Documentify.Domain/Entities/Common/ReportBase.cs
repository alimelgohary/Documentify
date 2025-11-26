
namespace Documentify.Domain.Entities.Common
{
    public partial class ReportBase
    {
        public Guid Id { get; set; }
        public DateTime ReportedAt { get; set; } = DateTime.UtcNow;
        public string Reason { get; set; } = null!;
        public bool IsResolved { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public string? Decision { get; set; }
        public string? ReporterId { get; set; }
        public string? ResolverId { get; set; }
    }
    public partial class ReportBase
    {
        public static class ValidationConstants
        {
            public const int ReasonMaxLength = 300;
            public const int DecisionMaxLength = 300;
        }
    }
}
