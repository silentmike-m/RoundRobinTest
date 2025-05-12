namespace Coda.RoundRobin.Infrastructure.Cache.Models;

using Coda.RoundRobin.Infrastructure.RoundRobin.Models;

internal sealed record EndpointsCacheKey : CacheKey<IReadOnlyList<Endpoint>>;
