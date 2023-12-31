using ICISAdminPortal.Application;
using ICISAdminPortal.Host.Configurations;
using ICISAdminPortal.Host.Controllers;
using ICISAdminPortal.Host.Middlewares;
using ICISAdminPortal.Infrastructure;
using ICISAdminPortal.Infrastructure.Common;
using ICISAdminPortal.Infrastructure.Logging.Serilog;
using Microsoft.Extensions.Options;
using Serilog;

[assembly: ApiConventionType(typeof(FSHApiConventions))]

StaticLogger.EnsureInitialized();
Log.Information("Server Booting Up...");
try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.AddConfigurations().RegisterSerilog();
    //builder.AddGlobalResourcesConfigurations();
    builder.Services.AddControllers();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddApplication();

    var app = builder.Build();

    await app.Services.InitializeDatabasesAsync();

    app.UseRequestLocalization(app.Services.GetService<IOptions<RequestLocalizationOptions>>().Value);
    app.UseInfrastructure(builder.Configuration);
    app.UseMiddleware<ErrorHandlerMiddleware>();
    app.MapEndpoints();
    app.Run();
}
catch (Exception ex) when (!ex.GetType().Name.Equals("StopTheHostException", StringComparison.Ordinal))
{
    StaticLogger.EnsureInitialized();
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    StaticLogger.EnsureInitialized();
    Log.Information("Server Shutting down...");
    Log.CloseAndFlush();
}