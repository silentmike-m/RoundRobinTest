using Coda.RoundRobin.Application;
using Coda.RoundRobin.Infrastructure;
using Coda.RoundRobin.WebApi.Constants;
using Coda.RoundRobin.WebApi.Extensions;
using Coda.RoundRobin.WebApi.Filters;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

const int EXIT_FAILURE = 1;
const int EXIT_SUCCESS = 0;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Host.UseSerilog((_, lc) => lc
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.WithProperty(nameof(ServiceConstants.ServiceName), ServiceConstants.ServiceName)
    .Enrich.WithProperty(nameof(ServiceConstants.ServiceVersion), ServiceConstants.ServiceVersion));

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

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration, builder.Environment);

builder.Services.ConfigureSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName);
});

builder.Services.AddSwaggerGen();

builder.Services.AddControllers(options => options.Filters.Add<ApiExceptionFilterAttribute>());

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

public partial class Program
{
}
