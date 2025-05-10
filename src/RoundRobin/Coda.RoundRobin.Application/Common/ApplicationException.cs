namespace Coda.RoundRobin.Application.Common;

public abstract class ApplicationException : Exception
{
    public abstract string Code { get; }

    protected ApplicationException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
