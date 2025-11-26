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
    public class ServiceSuggestionConfiguration : IEntityTypeConfiguration<ServiceSuggestion>
    {
        public void Configure(EntityTypeBuilder<ServiceSuggestion> builder)
        {
            builder.Property(x => x.Change)
                .HasMaxLength(ServiceSuggestion.ValidationConstants.ChangeMaxLength);

            builder.HasOne(x => x.Service)
                .WithMany(x => x.ServiceSuggestions)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);
        }
    }
}
