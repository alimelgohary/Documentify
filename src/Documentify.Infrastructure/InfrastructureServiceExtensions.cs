using Documentify.ApplicationCore;
using Documentify.ApplicationCore.Common.Interfaces;
using Documentify.ApplicationCore.Mail;
using Documentify.ApplicationCore.Repository;
using Documentify.Infrastructure.BackgroundTasks;
using Documentify.Infrastructure.Data;
using Documentify.Infrastructure.Identity;
using Documentify.Infrastructure.Identity.Entities;
using Documentify.Infrastructure.Mail;
using Documentify.Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;
namespace Documentify.Infrastructure
{
    public static class InfrastructureServiceExtensions
    {
        public static class ConfigurationKeys
        {
            public const string JwtSecret = "Jwt:Key";
            public const string JwtRefreshSecret = "Jwt:RefreshSecret";
            public const string JwtIssuer = "Jwt:Issuer";
            public const string JwtExpiryMinutes = "Jwt:ExpiryMinutes";
            public const string JwtRefreshExpiryMinutes = "Jwt:RefreshExpiryMinutes";

            public const string GoogleClientId = "Authentication:Google:ClientId";
            public const string GoogleClientSecret = "Authentication:Google:ClientSecret";

            public const string DocumentifyConnectionString = "DocumentifyConnection";

            public const string MaxRequestTimeWarningThreshold = "MaxRequestTimeWarningThreshold";

            public const string GmailUser = "gmail_user";
            public const string GmailPass = "gmail_password";
        }
        static void ValidateConfiguration(IConfiguration configuration, ILogger logger)
        {
            var fields = typeof(ConfigurationKeys).GetFields();
            List<string> errors = new List<string>(fields.Length);
            
            logger.LogInformation("Validating infrastructure configuration");
            foreach(var field in fields)
            {
                var key = field.GetValue(null)!.ToString()!;
                logger.LogDebug("Checking configuration key: {key}", key);
               
                if (string.IsNullOrEmpty(configuration[key!]))
                    errors.Add(key);
            }
            if (errors.Any())
            {
                throw new Exception("Missing required configuration setting: " + string.Join(",", errors));
            }
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection serviceCollection,
            IConfiguration configuration,
            IHostEnvironment environment,
            ILogger logger)
        {
            logger.LogInformation("Configuring infrastructure services");
            
            ValidateConfiguration(configuration, logger);

            serviceCollection.AddDatabase(configuration, environment, logger)
                             .AddUow(logger)
                             .AddIdentityAndAuthentication(configuration, environment, logger)
                             .AddHostedServices()
                             .AddMailServices(environment)
                             .AddMemoryCache();
            return serviceCollection;
        }
        static IServiceCollection AddDatabase(this IServiceCollection serviceCollection,
            IConfiguration configuration,
            IHostEnvironment environment,
            ILogger logger)
        {
            logger.LogInformation("Configuring database connection");
            string connectionString = configuration[ConfigurationKeys.DocumentifyConnectionString]!;

            serviceCollection.AddDbContext<AppDbContext>(
                opt => opt.UseSqlServer(connectionString));

            serviceCollection.AddScoped<ISeedDatabase, SeedDatabase>();
            return serviceCollection;
        }
        static IServiceCollection AddUow(this IServiceCollection serviceCollection,
            ILogger logger)
        {
            logger.LogInformation("Registering unit of work");
            serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();
            return serviceCollection;
        }
        static IServiceCollection AddIdentityAndAuthentication(this IServiceCollection serviceCollection,
            IConfiguration configuration,
            IHostEnvironment environment,
            ILogger logger)
        {
            logger.LogInformation("Configuring identity");
            serviceCollection.AddIdentity<ApplicationUser, IdentityRole>(opt =>
            {
                opt.Password.RequireDigit = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequiredLength = 8;
                opt.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<AppDbContext>()
              .AddDefaultTokenProviders();

            serviceCollection.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration[ConfigurationKeys.JwtIssuer],
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration[ConfigurationKeys.JwtSecret]!))
                };
            }).AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = configuration[ConfigurationKeys.GoogleClientId]!;
                googleOptions.ClientSecret = configuration[ConfigurationKeys.GoogleClientSecret]!;
                googleOptions.Scope.Add("profile");
                googleOptions.Scope.Add("email");
            });

            serviceCollection.AddScoped<IExternalAuthService, ExternalAuthService>();
            serviceCollection.AddScoped<IAuthService, AuthService>();
            serviceCollection.AddScoped<ITokenGenerator, JwtTokenGenerator>();
            return serviceCollection;
        }

        static IServiceCollection AddHostedServices(this IServiceCollection services)
        {
            services.AddHostedService<RevokedRefreshTokensCleanup>();
            return services;
        }

        static IServiceCollection AddMailServices(this IServiceCollection services, IHostEnvironment env)
        {
            if (env.IsProduction())
                services.AddScoped<IMailService, GmailSmtpService>();
            else
                services.AddScoped<IMailService, DummyMailService>();
            return services;
        }
    }
}
