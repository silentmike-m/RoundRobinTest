namespace Coda.RoundRobin.Infrastructure.HealthChecks.Publishers;

using Coda.RoundRobin.Infrastructure.RoundRobin.Enums;
using Coda.RoundRobin.Infrastructure.RoundRobin.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;

internal sealed class EndpointsHealthCheckPublisher : IHealthCheckPublisher
{
    private readonly IEndpointResolver endpointResolver;

    public EndpointsHealthCheckPublisher(IEndpointResolver endpointResolver)
        => this.endpointResolver = endpointResolver;

    public async Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
    {
        var endpoints = report.Entries
            .ToDictionary(entry => entry.Key, entry => MapStatus(entry.Value.Status));

        await this.endpointResolver.UpdateEndpointsAsync(endpoints, cancellationToken);
    }

    private static EndpointStatus MapStatus(HealthStatus status)
        => status switch
        {
            HealthStatus.Degraded or HealthStatus.Unhealthy => EndpointStatus.Unhealthy,
            HealthStatus.Healthy => EndpointStatus.Healthy,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, message: null),
        };
}
