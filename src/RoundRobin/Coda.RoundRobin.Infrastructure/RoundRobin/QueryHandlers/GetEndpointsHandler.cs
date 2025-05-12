namespace Coda.RoundRobin.Infrastructure.RoundRobin.QueryHandlers;

using Coda.RoundRobin.Application.RoundRobin.Dto;
using Coda.RoundRobin.Application.RoundRobin.Queries;
using Coda.RoundRobin.Infrastructure.RoundRobin.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

internal sealed class GetEndpointsHandler : IRequestHandler<GetEndpoints, IReadOnlyList<EndpointDto>>
{
    private readonly IEndpointResolver endpointResolver;
    private readonly ILogger<GetEndpointsHandler> logger;

    public GetEndpointsHandler(IEndpointResolver endpointResolver, ILogger<GetEndpointsHandler> logger)
    {
        this.endpointResolver = endpointResolver;
        this.logger = logger;
    }

    public async Task<IReadOnlyList<EndpointDto>> Handle(GetEndpoints request, CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Try to get endpoints");

        var endpoints = await this.endpointResolver.GetEndpointsAsync(cancellationToken);

        var result = endpoints
            .Select(endpoint => new EndpointDto
            {
                Name = endpoint.Name,
                Status = endpoint.Status.ToString(),
                Uri = endpoint.Uri,
            }).ToList();

        return result;
    }
}
