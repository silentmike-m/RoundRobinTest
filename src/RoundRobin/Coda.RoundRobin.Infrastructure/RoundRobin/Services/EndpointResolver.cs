namespace Coda.RoundRobin.Infrastructure.RoundRobin.Services;

using Coda.RoundRobin.Infrastructure.Cache.Interfaces;
using Coda.RoundRobin.Infrastructure.Cache.Models;
using Coda.RoundRobin.Infrastructure.RoundRobin.Enums;
using Coda.RoundRobin.Infrastructure.RoundRobin.Exception;
using Coda.RoundRobin.Infrastructure.RoundRobin.Interfaces;
using Coda.RoundRobin.Infrastructure.RoundRobin.Models;
using Microsoft.Extensions.Options;

internal sealed class EndpointResolver : IEndpointResolver
{
    private readonly ICacheService cacheService;
    private readonly RoundRobinOptions roundRobinOptions;

    public EndpointResolver(ICacheService cacheService, IOptions<RoundRobinOptions> roundRobinOptions)
    {
        this.cacheService = cacheService;
        this.roundRobinOptions = roundRobinOptions.Value;
    }

    public async Task<IReadOnlyList<Endpoint>> GetEndpointsAsync(CancellationToken cancellationToken)
    {
        var cacheKey = new EndpointsCacheKey();

        return await this.GetEndpointsAsync(cacheKey, cancellationToken);
    }

    public async Task<Endpoint> GetNextEndpointAsync(CancellationToken cancellationToken)
    {
        var cacheKey = new CurrentEndpointIndexCacheKey();

        var index = await this.cacheService.GetAsync(cacheKey, cancellationToken);

        var endpoints = await this.GetEndpointsAsync(cancellationToken);

        var result = GetNextEndpoint(endpoints, index);

        index = endpoints.ToList().IndexOf(result);

        await this.cacheService.SetAsync(cacheKey, index, keyTimeoutInMinutes: null, cancellationToken);

        return result;
    }

    public async Task InitializeEndpointsAsync(CancellationToken cancellationToken)
    {
        var endpoints = this.roundRobinOptions.Endpoints
            .Select(endpoint => new Endpoint
            {
                Name = endpoint.Key,
                Status = EndpointStatus.Healthy,
                Uri = endpoint.Value,
            }).ToList();

        var cacheKey = new EndpointsCacheKey();

        await this.UpdateEndpointsAsync(cacheKey, endpoints, cancellationToken);
    }

    public async Task UpdateEndpointAsync(Endpoint endpointToUpdate, EndpointStatus status, CancellationToken cancellationToken)
    {
        var cacheKey = new EndpointsCacheKey();
        var endpoints = await this.GetEndpointsAsync(cacheKey, cancellationToken);

        var endpoint = endpoints.FirstOrDefault(endpoint => endpoint.Name.Equals(endpointToUpdate.Name, StringComparison.OrdinalIgnoreCase));

        if (endpoint is not null)
        {
            endpoint.Status = status;

            await this.UpdateEndpointsAsync(cacheKey, endpoints, cancellationToken);
        }
        else
        {
            var newEndpoints = new List<Endpoint>(endpoints)
            {
                endpointToUpdate,
            };

            await this.UpdateEndpointsAsync(cacheKey, newEndpoints, cancellationToken);
        }
    }

    public async Task UpdateEndpointsAsync(IReadOnlyDictionary<string, EndpointStatus> endpointsStatus, CancellationToken cancellationToken)
    {
        var cacheKey = new EndpointsCacheKey();

        var endpoints = await this.GetEndpointsAsync(cacheKey, cancellationToken);

        foreach (var endpoint in endpoints)
        {
            endpoint.Status = endpointsStatus.GetValueOrDefault(endpoint.Name, EndpointStatus.Unhealthy);
        }

        await this.UpdateEndpointsAsync(cacheKey, endpoints, cancellationToken);
    }

    private async Task<IReadOnlyList<Endpoint>> GetEndpointsAsync(EndpointsCacheKey cacheKey, CancellationToken cancellationToken)
    {
        var result = await this.cacheService.GetAsync(cacheKey, cancellationToken);

        return result ?? [];
    }

    private async Task UpdateEndpointsAsync(EndpointsCacheKey cacheKey, IReadOnlyList<Endpoint> endpoints, CancellationToken cancellationToken)
        => await this.cacheService.SetAsync(cacheKey, endpoints, keyTimeoutInMinutes: null, cancellationToken);

    private static Endpoint GetNextEndpoint(IReadOnlyList<Endpoint> endpoints, int currentIndex)
    {
        var healthyEndpoints = endpoints
            .Where(endpoint => endpoint.Status is EndpointStatus.Healthy)
            .ToList();

        if (healthyEndpoints.Count == 0)
        {
            throw new HealthyEndpointNotFoundException();
        }

        currentIndex++;

        if (currentIndex >= healthyEndpoints.Count)
        {
            currentIndex = 0;
        }

        return healthyEndpoints.Skip(currentIndex).First();
    }
}
