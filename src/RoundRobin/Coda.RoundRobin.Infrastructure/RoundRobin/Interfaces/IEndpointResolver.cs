namespace Coda.RoundRobin.Infrastructure.RoundRobin.Interfaces;

using Coda.RoundRobin.Infrastructure.RoundRobin.Enums;
using Coda.RoundRobin.Infrastructure.RoundRobin.Models;

internal interface IEndpointResolver
{
    public Task<IReadOnlyList<Endpoint>> GetEndpointsAsync(CancellationToken cancellationToken);
    public Task<Endpoint> GetNextEndpointAsync(CancellationToken cancellationToken);
    public Task InitializeEndpointsAsync(CancellationToken cancellationToken);
    public Task UpdateEndpointAsync(Endpoint endpointToUpdate, EndpointStatus status, CancellationToken cancellationToken);
    public Task UpdateEndpointsAsync(IReadOnlyDictionary<string, EndpointStatus> endpointsStatus, CancellationToken cancellationToken);
}
