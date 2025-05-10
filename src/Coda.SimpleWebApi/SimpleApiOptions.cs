namespace Coda.SimpleWebApi;

public sealed record SimpleApiOptions
{
    public bool ThrowExceptions { get; init; } = false;
    public int WaitTimeInSeconds { get; init; } = 0;
};
