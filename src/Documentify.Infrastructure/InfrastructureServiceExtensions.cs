using Documentify.ApplicationCore.Repository;
using Documentify.Infrastructure.Data;
using Documentify.Infrastructure.Identity.Entities;
using Documentify.Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Documentify.Infrastructure
{
    public static class InfrastructureServiceExtensions
    {
        private static readonly string connectionStringKey = "connectionStrings:DocumentifyConnection";
        public static IServiceCollection AddInfrastructure(this IServiceCollection serviceCollection,
            IConfiguration configuration,
            IHostEnvironment environment,
            ILogger logger = null) // TODO: Add logging and remove default value
        {
            serviceCollection.AddDatabase(configuration, environment, logger);

            serviceCollection.AddRepositories(logger);

            serviceCollection.AddIdentity<ApplicationUser, IdentityRole>(opt => 
            {
                opt.Password.RequireDigit = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequiredLength = 8;
                opt.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<AppDbContext>();
            
            return serviceCollection;
        }
        public static IServiceCollection AddDatabase(this IServiceCollection serviceCollection,
            IConfiguration configuration,
            IHostEnvironment environment,
            ILogger logger = null)
        {
            string? connectionString = null;
            if (environment.IsDevelopment())
            {
                connectionString = configuration[connectionStringKey];
            }
            else
            {
                connectionString = Environment.GetEnvironmentVariable(connectionStringKey);
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                logger?.LogCritical("Unable to find connection string with key: {connectionStringKey}", connectionString);
                Environment.Exit(1);
            }
            serviceCollection.AddDbContext<AppDbContext>(
                opt => opt.UseSqlServer(connectionString));
            return serviceCollection;
        }
        public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection,
            ILogger logger = null)
        {
            serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();
            return serviceCollection;
        }
    }
}
