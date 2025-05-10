namespace Coda.RoundRobin.Infrastructure.RoundRobin.Interfaces;

internal interface IEndpointResolver
{
    public Task<Uri> GetNextEndpointAsync(CancellationToken cancellationToken);
}
