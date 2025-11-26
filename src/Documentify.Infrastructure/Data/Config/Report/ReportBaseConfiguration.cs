using Documentify.Domain.Entities.Common;
using Documentify.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Documentify.Infrastructure.Data.Config.Report
{
    public class ReportBaseConfiguration : IEntityTypeConfiguration<ReportBase>
    {
        public void Configure(EntityTypeBuilder<ReportBase> builder)
        {
            builder.Property(x => x.Reason)
                .HasMaxLength(ReportBase.ValidationConstants.ReasonMaxLength);

            builder.Property(x => x.Decision)
                .HasMaxLength(ReportBase.ValidationConstants.DecisionMaxLength);

            builder.HasOne<ApplicationUser>()
                .WithMany(x => x.Reports)
                .HasForeignKey(x => x.ReporterId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            builder.HasOne<ApplicationUser>()
                .WithMany(x => x.ResolvedReports)
                .HasForeignKey(x => x.ResolverId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);
        }
    }
}
