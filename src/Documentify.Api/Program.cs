using Documentify.Api;
using Serilog;
using Serilog.Extensions.Logging;
try
{
    var builder = WebApplication.CreateBuilder(args);

    // Configure Serilog and create a logger
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .CreateLogger();
    Log.Information("Starting web application");
    var appLogger = new SerilogLoggerFactory(Log.Logger).CreateLogger<Program>();

    builder.Services.AddApiServices(builder.Configuration, builder.Environment, appLogger);
    
    var app = builder.Build();
    
    app.UseApi(app.Environment, appLogger);

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}