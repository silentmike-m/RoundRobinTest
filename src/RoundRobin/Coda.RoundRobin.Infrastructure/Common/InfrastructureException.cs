namespace Coda.RoundRobin.Infrastructure.Common;

public abstract class InfrastructureException : Exception
{
    public abstract string Code { get; }

    protected InfrastructureException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
