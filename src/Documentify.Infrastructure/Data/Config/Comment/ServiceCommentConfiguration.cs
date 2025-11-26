using Documentify.Domain.Entities.Comment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Documentify.Infrastructure.Data.Config.Comment
{
    public class ServiceCommentConfiguration : IEntityTypeConfiguration<ServiceComment>
    {
        public void Configure(EntityTypeBuilder<ServiceComment> builder)
        {
            builder.HasOne(sc => sc.Service)
                    .WithMany(s => s.ServiceComments)
                    .HasForeignKey(sc => sc.ServiceId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired(true);
        }
    }
}
