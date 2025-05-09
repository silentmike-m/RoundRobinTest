namespace Coda.RoundRobin.Infrastructure.RoundRobin;

internal sealed record RoundRobinOptions
{
    public IReadOnlyList<Uri> Endpoints { get; init; } = new List<Uri>();
};
