using Documentify.Api.Middlewares;
using Serilog;
using Serilog.Events;

namespace Documentify.Api
{
    public static class AppExtensions
    {
        public static WebApplication UseApi(this WebApplication app, IHostEnvironment environment, Microsoft.Extensions.Logging.ILogger logger)
        {
            app.UseSerilogRequestMiddleware();
            app.UseMiddleware<IdempotencyMiddleware>();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseMiddleware<ExceptionHandlerMiddleware>();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            return app;
        }
        public static void UseSerilogRequestMiddleware(this WebApplication app)
        {
            app.UseSerilogRequestLogging(options =>
            {
                var maxRequestTimeThreshold = double.Parse(app.Configuration["MaxRequestTimeWarningThreshold"]);
                options.GetLevel = (httpContext, elapsed, ex)
                                    => elapsed > maxRequestTimeThreshold ? LogEventLevel.Warning : LogEventLevel.Information;
                options.MessageTemplate = "{RequestScheme} {User} {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme.ToUpper());
                    diagnosticContext.Set("User", httpContext.User.Identity?.Name ?? "Anonymous");
                    diagnosticContext.Set("SourceContext", "");
                };
            });
        }
    }
}
