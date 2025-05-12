namespace Coda.RoundRobin.Infrastructure.HealthChecks;

using System.Diagnostics.CodeAnalysis;
using Coda.RoundRobin.Infrastructure.Cache;
using Coda.RoundRobin.Infrastructure.HealthChecks.Constants;
using Coda.RoundRobin.Infrastructure.HealthChecks.Publishers;
using Coda.RoundRobin.Infrastructure.RoundRobin;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

[ExcludeFromCodeCoverage]
internal static class DependencyInjection
{
    private const int HEALTH_CHECK_PUBLISH_DELAY_IN_SECONDS = 2;

    public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var roundRobinOptions = configuration.GetSection(nameof(RoundRobinOptions)).Get<RoundRobinOptions>();
        roundRobinOptions ??= new RoundRobinOptions();

        services
            .AddHealthChecks()
            .AddSimpleApis(roundRobinOptions)
            .AddRedis(serviceProvider =>
            {
                var options = serviceProvider.GetRequiredService<RedisOptions>();

                return options.ConnectionString;
            }, failureStatus: HealthStatus.Degraded);

        services.Configure<HealthCheckPublisherOptions>(options =>
        {
            options.Delay = TimeSpan.FromSeconds(HEALTH_CHECK_PUBLISH_DELAY_IN_SECONDS);
            options.Predicate = healthCheck => healthCheck.Tags.Contains(Tags.SIMPLE_API_HEALTH_CHECK_TAG);
        });

        services.AddSingleton<IHealthCheckPublisher, EndpointsHealthCheckPublisher>();

        return services;
    }

    private static IHealthChecksBuilder AddSimpleApis(this IHealthChecksBuilder builder, RoundRobinOptions options)
    {
        foreach (var (endpointName, endpointUri) in options.Endpoints)
        {
            builder.AddUrlGroup(endpointUri, endpointName, HealthStatus.Degraded, [Tags.SIMPLE_API_HEALTH_CHECK_TAG]);
        }

        return builder;
    }
}
