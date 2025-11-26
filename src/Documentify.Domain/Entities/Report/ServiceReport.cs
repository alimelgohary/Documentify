using Documentify.Domain.Entities.Common;

namespace Documentify.Domain.Entities.Report
{
    public class ServiceReport : ReportBase
    {
        public Guid ServiceId { get; set; }
        public Service Service { get; set; } = null!;
    }
}
