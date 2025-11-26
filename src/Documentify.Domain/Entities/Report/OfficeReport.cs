using Documentify.Domain.Entities.Common;

namespace Documentify.Domain.Entities.Report
{
    public class OfficeReport : ReportBase
    {
        public Guid OfficeId { get; set; }
        public Office Office { get; set; } = null!;
    }
}
