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
    public class OfficeSuggestionConfiguration : IEntityTypeConfiguration<OfficeSuggestion>
    {
        public void Configure(EntityTypeBuilder<OfficeSuggestion> builder)
        {

            builder.Property(x => x.Change)
                .HasMaxLength(OfficeSuggestion.ValidationConstants.ChangeMaxLength);

            builder.HasOne(x => x.Office)
                    .WithMany(x => x.OfficeSuggestions)
                    .HasForeignKey(x => x.OfficeId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .IsRequired(false);
        }
    }
}
