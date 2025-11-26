using Documentify.Domain.Entities.Comment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Documentify.Infrastructure.Data.Config.Comment
{
    public class OfficeSuggestionCommentConfiguration : IEntityTypeConfiguration<OfficeSuggestionComment>
    {
        public void Configure(EntityTypeBuilder<OfficeSuggestionComment> builder)
        {
            builder.HasOne(osc => osc.OfficeSuggestion)
                    .WithMany(os => os.OfficeSuggestionComments)
                    .HasForeignKey(osc => osc.OfficeSuggestionId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired(true);
        }
    }
}
