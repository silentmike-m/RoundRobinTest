namespace Coda.RoundRobin.Infrastructure.RoundRobin.Services;

using Coda.RoundRobin.Infrastructure.Cache.Interfaces;
using Coda.RoundRobin.Infrastructure.Cache.Models;
using Coda.RoundRobin.Infrastructure.RoundRobin.Interfaces;
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

    public async Task<Uri> GetNextEndpointAsync(CancellationToken cancellationToken)
    {
        var cacheKey = new CurrentEndpointIndexCacheKey();

        var index = await this.cacheService.GetAsync(cacheKey, cancellationToken);

        var result = this.roundRobinOptions.Endpoints[index];

        if (index == this.roundRobinOptions.Endpoints.Count - 1)
        {
            index = 0;
        }
        else
        {
            index++;
        }

        await this.cacheService.SetAsync(cacheKey, index, TimeSpan.MaxValue, cancellationToken);

        return result;
    }
}
