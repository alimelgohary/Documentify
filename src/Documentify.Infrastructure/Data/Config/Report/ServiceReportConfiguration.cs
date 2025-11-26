using Documentify.Domain.Entities.Report;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Documentify.Infrastructure.Data.Config.Report
{
    public class ServiceReportConfiguration : IEntityTypeConfiguration<ServiceReport>
    {
        public void Configure(EntityTypeBuilder<ServiceReport> builder)
        {
            builder.HasOne(sr => sr.Service)
                    .WithMany(s => s.Reports)
                    .HasForeignKey(sr => sr.ServiceId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired(true);
        }
    }
}
