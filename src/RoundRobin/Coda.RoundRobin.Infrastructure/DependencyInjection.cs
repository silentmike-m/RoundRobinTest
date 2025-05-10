namespace Coda.RoundRobin.Infrastructure;

using System.Diagnostics.CodeAnalysis;
using Coda.RoundRobin.Infrastructure.Cache;
using Coda.RoundRobin.Infrastructure.HealthChecks;
using Coda.RoundRobin.Infrastructure.RoundRobin;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services
            .AddHealthChecks(configuration)
            .AddCache(configuration, environment)
            .AddRoundRobin(configuration);

        return services;
    }
}
