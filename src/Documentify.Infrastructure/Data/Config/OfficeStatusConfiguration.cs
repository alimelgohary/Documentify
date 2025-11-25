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
    public class OfficeStatusConfiguration : IEntityTypeConfiguration<OfficeStatus>
    {
        public void Configure(EntityTypeBuilder<OfficeStatus> builder)
        {
            builder.Property(os => os.Description)
                .HasMaxLength(OfficeStatus.ValidationConstants.DescriptionMaxLength);
        }
    }
}
