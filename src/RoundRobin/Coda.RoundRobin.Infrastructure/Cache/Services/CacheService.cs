namespace Coda.RoundRobin.Infrastructure.Cache.Services;

using System.Text;
using Coda.RoundRobin.Application.Extensions;
using Coda.RoundRobin.Infrastructure.Cache.Interfaces;
using Coda.RoundRobin.Infrastructure.Cache.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

internal sealed class CacheService : ICacheService
{
    private readonly IDistributedCache cache;
    private readonly ILogger<CacheService> logger;

    public CacheService(IDistributedCache cache, ILogger<CacheService> logger)
    {
        this.cache = cache;
        this.logger = logger;
    }

    public async Task ClearAsync<TResponse>(CacheKey<TResponse> key, CancellationToken cancellationToken)
    {
        try
        {
            var keyString = key.ToString();

            await this.cache.RemoveAsync(keyString, cancellationToken);
        }
        catch (Exception exception)
        {
            this.logger.LogWarning(exception, "Error on clear cache key");
        }
    }

    public async Task<TResponse?> GetAsync<TResponse>(CacheKey<TResponse> key, CancellationToken cancellationToken)
    {
        try
        {
            var keyString = key.ToString();
            var bytes = await this.cache.GetAsync(keyString, cancellationToken);

            if (bytes is null)
            {
                return default;
            }

            var json = Encoding.UTF8.GetString(bytes);
            var value = json.To<TResponse>();

            return value;
        }
        catch (Exception exception)
        {
            this.logger.LogWarning(exception, "Error on get cache key value");

            return default;
        }
    }

    public async Task SetAsync<TResponse>(CacheKey<TResponse> key, TResponse value, int keyTimeoutInMinutes, CancellationToken cancellationToken)
        => await this.SetAsync(key, value, TimeSpan.FromMinutes(keyTimeoutInMinutes), cancellationToken);

    public async Task SetAsync<TResponse>(CacheKey<TResponse> key, TResponse value, TimeSpan keyTimeout, CancellationToken cancellationToken)
    {
        try
        {
            var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(keyTimeout);

            var json = value.ToJson();
            var bytes = Encoding.UTF8.GetBytes(json);

            var keyString = key.ToString();

            await this.cache.SetAsync(keyString, bytes, options, cancellationToken);
        }
        catch (Exception exception)
        {
            this.logger.LogWarning(exception, "Error on set cache key");
        }
    }
}
