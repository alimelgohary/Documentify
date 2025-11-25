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
    internal class UserRateOfficeConfiguration : IEntityTypeConfiguration<UserRateOffice>
    {
        public void Configure(EntityTypeBuilder<UserRateOffice> builder)
        {
            builder.Property(x => x.Comment)
                .HasMaxLength(UserRateOffice.ValidationConstants.CommentMaxLength);
        }
    }
}
