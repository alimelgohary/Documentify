using Documentify.ApplicationCore;
using Documentify.ApplicationCore.Common.Behaviors;
using MediatR;
using Documentify.Infrastructure;
using FluentValidation;
using Documentify.Api.Swagger;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Serilog;
namespace Documentify.Api
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services,
                                                             IConfiguration configuration,
                                                             IHostEnvironment environment,
                                                             Microsoft.Extensions.Logging.ILogger logger
            )
        {
            services.AddControllers();
            services.AddSerilogLogging()
                    .AddSwagger()
                    .AddInfrastructure(configuration, environment, logger)
                    .AddValidatorsFromAssembly(typeof(IApplicationCoreMarker).Assembly)
                    .AddMediator()
                    .AddMemoryCache();

            return services;
        }
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddFluentValidationRulesToSwagger();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Documentify API",
                    Version = "v1.0",
                    Description = "A personalized digital assistant that demystifies government procedures, providing a clear roadmap for every administrative task",
                    Contact = new OpenApiContact { Name = "Support", Email = "Ali.Algohary@outlook.com" }
                });

                // Add JWT Auth
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [your token]"
                });

                c.OperationFilter<SecurityRequirementsOperationFilter>();
            });
            return services;
        }
        public static IServiceCollection AddSerilogLogging(this IServiceCollection services)
        {
            services.AddSerilog();
            return services;
        }
        public static IServiceCollection AddMediator(this IServiceCollection services)
        {
            services.AddMediatR(
                cfg => cfg.RegisterServicesFromAssemblyContaining<IApplicationCoreMarker>());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            return services;
        }
    }
}
