using Documentify.Domain.Entities.Report;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Documentify.Infrastructure.Data.Config.Report
{
    public class OfficeReportConfiguration : IEntityTypeConfiguration<OfficeReport>
    {
        public void Configure(EntityTypeBuilder<OfficeReport> builder)
        {
            builder.HasOne(or => or.Office)
                    .WithMany(o => o.Reports)
                    .HasForeignKey(or => or.OfficeId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired(true);
        }
    }
}
