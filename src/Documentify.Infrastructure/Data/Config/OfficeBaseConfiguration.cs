using Documentify.Domain.Entities;
using Documentify.Domain.Entities.Common;
using Documentify.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Documentify.Infrastructure.Data.Config
{
    public class OfficeBaseConfiguration : IEntityTypeConfiguration<OfficeBase>
    {
        public void Configure(EntityTypeBuilder<OfficeBase> builder)
        {
            builder.Property(o => o.Name)
                .HasMaxLength(Office.ValidationConstants.NameMaxLength);

            builder.Property(o => o.LastUpdated);

            builder.Property(o => o.Workdays);

            builder.Property(o => o.OpenFrom);

            builder.Property(o => o.OpenTo);

            builder.Property(o => o.EveningOpenFrom);

            builder.Property(o => o.EveningOpenTo);

            builder.Property(o => o.LocationLat);

            builder.Property(o => o.LocationLng);

            builder.Property(o => o.LocationText)
                    .HasMaxLength(OfficeBase.ValidationConstants.LocationTextMaxLength);

            builder.Property(o => o.Is24_7);

            builder.Property(o => o.Phones)
                    .HasMaxLength(OfficeBase.ValidationConstants.PhonesMaxLength * 11)
                     .HasConversion(
                         x => x == null ? null : string.Join(",", x),
                         x => x == null ? null : x.Split(",", StringSplitOptions.None),
                         new ValueComparer<string[]?>(
                             (ph1, ph2) => ReferenceEquals(ph1, ph2) ||
                                        (ph1 != null && ph2 != null && ph1.SequenceEqual(ph2)),
                                ph => ph == null ? 0 : ph.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                                ph => ph == null ? null : ph.ToArray()
                            )
                     );

            builder.HasOne<ApplicationUser>()
                .WithMany(u => u.OfficesWritten)
                .HasForeignKey(o => o.WriterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .IsRequired(false);

        }
    }
}
