namespace Coda.RoundRobin.Infrastructure;

using System.Diagnostics.CodeAnalysis;
using Coda.RoundRobin.Infrastructure.HealthChecks;
using Coda.RoundRobin.Infrastructure.RoundRobin;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHealthChecks(configuration)
            .AddRoundRobin(configuration);

        return services;
    }
}
