using Documentify.Domain.Entities;
using Documentify.Infrastructure.Identity;
using Documentify.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Documentify.Infrastructure.Data.Config
{
    public class AdminBanUserConfiguration : IEntityTypeConfiguration<AdminBanUser>
    {
        public void Configure(EntityTypeBuilder<AdminBanUser> builder)
        {
            builder.HasKey(abu => abu.UserId );

            builder.Property(abu => abu.Reason)
                .HasMaxLength(AdminBanUser.ValidationConstants.ReasonMaxLength);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(abu => abu.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(true);
            
            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(abu => abu.AdminId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

        }
    }
}
