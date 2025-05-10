namespace Coda.RoundRobin.Infrastructure.Cache.Services;

using Coda.RoundRobin.Infrastructure.Cache.Interfaces;
using Coda.RoundRobin.Infrastructure.Cache.Models;
using Microsoft.Extensions.Caching.Memory;

internal sealed class CacheServiceMock : ICacheService
{
    private readonly IMemoryCache memoryCache;

    public CacheServiceMock(IMemoryCache memoryCache)
        => this.memoryCache = memoryCache;

    public async Task ClearAsync<TResponse>(CacheKey<TResponse> key, CancellationToken cancellationToken)
    {
        var keyString = key.ToString();

        this.memoryCache.Remove(keyString);

        await Task.CompletedTask;
    }

    public async Task<TResponse?> GetAsync<TResponse>(CacheKey<TResponse> key, CancellationToken cancellationToken)
    {
        var keyString = key.ToString();

        var result = this.memoryCache.Get<TResponse>(keyString);

        result ??= default;

        return await Task.FromResult(result);
    }

    public async Task SetAsync<TResponse>(CacheKey<TResponse> key, TResponse value, int keyTimeoutInMinutes, CancellationToken cancellationToken)
        => await this.SetAsync(key, value, TimeSpan.FromMinutes(keyTimeoutInMinutes), cancellationToken);

    public async Task SetAsync<TResponse>(CacheKey<TResponse> key, TResponse value, TimeSpan keyTimeout, CancellationToken cancellationToken)
    {
        var keyString = key.ToString();

        this.memoryCache.Set(keyString, value, keyTimeout);

        await Task.CompletedTask;
    }
}
