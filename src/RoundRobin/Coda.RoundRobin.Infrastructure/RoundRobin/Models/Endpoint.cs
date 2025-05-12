namespace Coda.RoundRobin.Infrastructure.RoundRobin.Models;

using Coda.RoundRobin.Infrastructure.RoundRobin.Enums;

internal sealed record Endpoint
{
    public required string Name { get; init; }
    public required EndpointStatus Status { get; set; }
    public required Uri Uri { get; init; }
}
