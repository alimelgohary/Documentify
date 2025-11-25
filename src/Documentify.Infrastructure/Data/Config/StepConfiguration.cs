using Documentify.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Documentify.Infrastructure.Data.Config
{
    public class StepConfiguration : IEntityTypeConfiguration<Step>
    {
        public void Configure(EntityTypeBuilder<Step> builder)
        {
            builder.HasKey(x => new { x.AssociatedServiceId, x.Order });
            builder.Property(s => s.Order);

            builder.Property(s => s.TimeRequired);

            builder.Property(s => s.CostRequired)
                .HasPrecision(10, 2);

            builder.Property(s => s.Details)
                .HasMaxLength(Step.ValidationConstants.DetailsMaxLength);

            builder.HasOne(x => x.InnerService)
                    .WithMany()
                    .HasForeignKey(x => x.InnerServiceId)
                    .IsRequired(false);
        }
    }
}
