using Documentify.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Documentify.Infrastructure.Data.Config
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasMany(x => x.UpvotedServiceSuggestions)
                .WithMany()
                .UsingEntity(j => j.ToTable("UserServiceSuggestionUpvotes"));

            builder.HasMany(x => x.UpvotedOfficeSuggestions)
                .WithMany()
                .UsingEntity(j => j.ToTable("UserOfficeSuggestionUpvotes"));


            // USER ==== OFFICE
            builder.HasMany(x => x.RatedOffices)
                .WithMany()
                .UsingEntity<UserRateOffice>(
                    j => j.HasOne(x => x.Office)
                            .WithMany(x => x.UserRateOffices)
                            .HasForeignKey(x => x.OfficeId)
                            .OnDelete(DeleteBehavior.Cascade)
                            .IsRequired(true),

                    j => j.HasOne<ApplicationUser>()
                    .WithMany(x => x.UserRateOffices)
                    .HasForeignKey(x => x.RaterId)
                     .OnDelete(DeleteBehavior.Cascade)
                     .IsRequired(true),

                    j => j.HasKey(uo => new { uo.OfficeId, uo.RaterId })
                );
        }
    }
}
