namespace Coda.RoundRobin.Application.RoundRobin.Dto;

public sealed record EndpointDto
{
    public required string Name { get; init; }
    public required string Status { get; init; }
    public required Uri Uri { get; init; }
}
