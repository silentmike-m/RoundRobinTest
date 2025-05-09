namespace Coda.RoundRobin.Infrastructure.RoundRobin.Services;

using Coda.RoundRobin.Infrastructure.RoundRobin.Interfaces;
using Microsoft.Extensions.Options;

internal sealed class EndpointResolver : IEndpointResolver
{
    private readonly RoundRobinOptions roundRobinOptions;
    private int currentIndex = 0;

    public EndpointResolver(IOptions<RoundRobinOptions> roundRobinOptions)
        => this.roundRobinOptions = roundRobinOptions.Value;

    public Uri GetNextEndpoint()
    {
        var result = this.roundRobinOptions.Endpoints[this.currentIndex];

        if (this.currentIndex == this.roundRobinOptions.Endpoints.Count - 1)
        {
            this.currentIndex = 0;
        }
        else
        {
            this.currentIndex++;
        }

        return result;
    }
}
