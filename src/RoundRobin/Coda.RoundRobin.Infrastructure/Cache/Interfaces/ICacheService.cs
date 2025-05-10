namespace Coda.RoundRobin.Infrastructure.Cache.Interfaces;

using Coda.RoundRobin.Infrastructure.Cache.Models;

internal interface ICacheService
{
    Task ClearAsync<TResponse>(CacheKey<TResponse> key, CancellationToken cancellationToken);
    Task<TResponse?> GetAsync<TResponse>(CacheKey<TResponse> key, CancellationToken cancellationToken);
    Task SetAsync<TResponse>(CacheKey<TResponse> key, TResponse value, int keyTimeoutInMinutes, CancellationToken cancellationToken);
    Task SetAsync<TResponse>(CacheKey<TResponse> key, TResponse value, TimeSpan keyTimeout, CancellationToken cancellationToken);
}
