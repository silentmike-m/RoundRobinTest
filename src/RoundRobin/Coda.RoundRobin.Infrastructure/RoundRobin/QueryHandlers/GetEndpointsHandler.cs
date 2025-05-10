namespace Coda.RoundRobin.Infrastructure.RoundRobin.QueryHandlers;

using Coda.RoundRobin.Application.RoundRobin.Queries;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal sealed class GetEndpointsHandler : IRequestHandler<GetEndpoints, IReadOnlyList<Uri>>
{
    private readonly ILogger<GetEndpointsHandler> logger;
    private readonly RoundRobinOptions roundRobinOptions;

    public GetEndpointsHandler(ILogger<GetEndpointsHandler> logger, IOptions<RoundRobinOptions> roundRobinOptions)
    {
        this.logger = logger;
        this.roundRobinOptions = roundRobinOptions.Value;
    }

    public async Task<IReadOnlyList<Uri>> Handle(GetEndpoints request, CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Try to get endpoints");

        return await Task.FromResult(this.roundRobinOptions.Endpoints);
    }
}
