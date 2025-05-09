namespace Coda.RoundRobin.Infrastructure.HealthChecks;

using System.Diagnostics.CodeAnalysis;
using Coda.RoundRobin.Infrastructure.RoundRobin;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[ExcludeFromCodeCoverage]
internal static class DependencyInjection
{
    public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var roundRobinOptions = configuration.GetSection(nameof(RoundRobinOptions)).Get<RoundRobinOptions>();
        roundRobinOptions ??= new RoundRobinOptions();

        services
            .AddHealthChecks()
            .AddUrlGroup(roundRobinOptions.Endpoints, "simple-apis");

        return services;
    }
}
