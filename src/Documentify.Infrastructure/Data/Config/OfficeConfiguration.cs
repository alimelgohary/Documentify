using Documentify.Domain.Entities;
using Documentify.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Documentify.Infrastructure.Data.Config
{
    public class OfficeConfiguration : IEntityTypeConfiguration<Office>
    {
        public void Configure(EntityTypeBuilder<Office> builder)
        {
            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(o => o.ApproverId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            builder.HasMany(x => x.OfficeStatuses)
                   .WithOne(x => x.Office)
                   .HasForeignKey(x => x.OfficeId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired(true);
        }
    }
}
