using Coda.SimpleWebApi.Constants;
using Coda.SimpleWebApi.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

const int EXIT_FAILURE = 1;
const int EXIT_SUCCESS = 0;
const string ENRICH_APP_INSTANCE_VERSION_NAME = "AppInstanceVersion";

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

var appInstanceVersion = builder.Configuration[ENRICH_APP_INSTANCE_VERSION_NAME] ?? string.Empty;

builder.Host.UseSerilog((_, lc) => lc
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.WithProperty(nameof(ServiceConstants.ServiceName), ServiceConstants.ServiceName)
    .Enrich.WithProperty(nameof(ServiceConstants.ServiceVersion), ServiceConstants.ServiceVersion)
    .Enrich.WithProperty(ENRICH_APP_INSTANCE_VERSION_NAME, appInstanceVersion));

builder.Services
    .AddProblemDetails(options =>
        options.CustomizeProblemDetails = ctx =>
        {
            ctx.ProblemDetails.Extensions.Add("trace_id", ctx.HttpContext.TraceIdentifier);
            ctx.ProblemDetails.Extensions.Add("request", $"{ctx.HttpContext.Request.Method} {ctx.HttpContext.Request.Path}");
            ctx.ProblemDetails.Extensions.Add("service_name", ServiceConstants.ServiceName);
            ctx.ProblemDetails.Extensions.Add("service_version", ServiceConstants.ServiceVersion);
        });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHealthChecks();

builder.Services.ConfigureSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName);
});

builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

try
{
    Log.Information("Starting host...");

    var app = builder.Build();

    app.MapHealthChecks("/hc",
        new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = HealthCheckExtensions.WriteResponse,
        });

    app.UseSwagger();
    app.UseSwaggerUI();

    app.MapControllers();

    await app.RunAsync();

    return EXIT_SUCCESS;
}
catch (Exception exception)
{
    Log.Fatal(exception, "Host terminated unexpectedly");

    return EXIT_FAILURE;
}
finally
{
    await Log.CloseAndFlushAsync();
}
