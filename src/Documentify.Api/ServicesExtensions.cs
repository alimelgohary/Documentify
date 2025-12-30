using Documentify.ApplicationCore;
using Documentify.ApplicationCore.Common.Behaviors;
using MediatR;
using Documentify.Infrastructure;
using FluentValidation;
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
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
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
