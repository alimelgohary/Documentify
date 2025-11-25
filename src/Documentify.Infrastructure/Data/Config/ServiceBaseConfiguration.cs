using Documentify.Domain.Entities;
using Documentify.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Documentify.Infrastructure.Data.Config
{
    public class ServiceBaseConfiguration : IEntityTypeConfiguration<ServiceBase>
    {
        public void Configure(EntityTypeBuilder<ServiceBase> builder)
        {
            builder.Property(s => s.Name)
                .HasMaxLength(Service.ValidationConstants.NameMaxLength);

            builder.Property(s => s.IsAvailableOnline);

            builder.Property(s => s.Description)
                .HasMaxLength(Service.ValidationConstants.DescriptionMaxLength);

            builder.Property(s => s.LastUpdated);

            builder.Property(s => s.Notes)
                .HasMaxLength(Service.ValidationConstants.NotesMaxLength);

            builder.Property(s => s.EstimatedTime);

            builder.Property(s => s.EstimatedCost)
                .HasPrecision(11, 2);

            builder.HasOne(s => s.Category)
                .WithMany(c => c.Services)
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(s => s.Steps)
                .WithOne(step => step.AssociatedService)
                .HasForeignKey(s => s.AssociatedServiceId);

            builder.HasMany(x => x.Offices)
                   .WithMany(x => x.Services)
                   .UsingEntity("ServiceOfficeAvailability");

            builder.HasOne<ApplicationUser>()
                .WithMany(u => u.ServicesWritten)
                .HasForeignKey(s => s.WriterId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
