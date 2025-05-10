namespace Coda.RoundRobin.Infrastructure.HealthChecks;

using System.Diagnostics.CodeAnalysis;
using Coda.RoundRobin.Infrastructure.Cache;
using Coda.RoundRobin.Infrastructure.RoundRobin;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

[ExcludeFromCodeCoverage]
internal static class DependencyInjection
{
    public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var roundRobinOptions = configuration.GetSection(nameof(RoundRobinOptions)).Get<RoundRobinOptions>();
        roundRobinOptions ??= new RoundRobinOptions();

        services
            .AddHealthChecks()
            .AddUrlGroup(roundRobinOptions.Endpoints, "simple-apis")
            .AddRedis(serviceProvider =>
            {
                var options = serviceProvider.GetRequiredService<RedisOptions>();

                return options.ConnectionString;
            }, failureStatus: HealthStatus.Degraded);

        return services;
    }
}
