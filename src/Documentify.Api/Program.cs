using Documentify.Api;
using Documentify.ApplicationCore;
using Serilog;
using Serilog.Extensions.Logging;
try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Configuration.AddJsonFile("secrets.json");
    // Configure Serilog and create a logger
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .CreateLogger();
    Log.Information("=========== Starting web application ===========");
    var appLogger = new SerilogLoggerFactory(Log.Logger).CreateLogger<Program>();
    
    builder.Services.AddApiServices(builder.Configuration, builder.Environment, appLogger);
    
    var app = builder.Build();
    
    app.UseApi(app.Environment, appLogger);

    var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<ISeedDatabase>().Seed();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal("Application terminated unexpectedly: " + ex.Message);
}
finally
{
    Log.CloseAndFlush();
}
public partial class Program { } // For integration tests referencing Program class