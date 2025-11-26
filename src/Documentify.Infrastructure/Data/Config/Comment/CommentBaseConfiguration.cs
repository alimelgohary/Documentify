using Documentify.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Documentify.Infrastructure.Data.Config.Comment
{
    public class CommentBaseConfiguration : IEntityTypeConfiguration<CommentBase>
    {
        public void Configure(EntityTypeBuilder<CommentBase> builder)
        {
            builder.UseTpcMappingStrategy();

            builder.Property(cb => cb.Comment)
                .HasMaxLength(CommentBase.ValidationConstants.CommentMaxLength);
        }
    }
}
