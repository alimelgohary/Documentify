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
    public class ServiceSuggestionCommentConfiguration : IEntityTypeConfiguration<ServiceSuggestionComment>
    {
        public void Configure(EntityTypeBuilder<ServiceSuggestionComment> builder)
        {
            builder.HasOne(ssc => ssc.ServiceSuggestion)
                    .WithMany(ss => ss.ServiceSuggestionComments)
                    .HasForeignKey(ssc => ssc.ServiceSuggestionId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired(true);
        }
    }
}
