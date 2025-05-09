namespace Coda.RoundRobin.Infrastructure.RoundRobin.Interfaces;

internal interface IEndpointResolver
{
    public Uri GetNextEndpoint();
}
