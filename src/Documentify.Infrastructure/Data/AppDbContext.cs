using Documentify.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Documentify.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceSuggestion> ServiceSuggestions { get; set; }
        public DbSet<Office> Offices { get; set; }
        public DbSet<OfficeSuggestion> OfficeSuggestions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Step> Steps { get; set; }
        //TODO: on delete


        public AppDbContext(DbContextOptions options) : base(options) {}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
