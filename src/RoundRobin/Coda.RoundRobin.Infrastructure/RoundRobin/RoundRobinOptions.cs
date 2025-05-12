namespace Coda.RoundRobin.Infrastructure.RoundRobin;

internal sealed record RoundRobinOptions
{
    public Dictionary<string, Uri> Endpoints { get; init; } = new();
    public int MaxRetries { get; init; } = 1;
    public int[] RetryDelayInSeconds { get; init; } = [1];
    public int RetryTimeoutInSeconds { get; init; } = 5;
};
