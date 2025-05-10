namespace Coda.RoundRobin.Infrastructure.RoundRobin.Exception;

using Coda.RoundRobin.Infrastructure.Common;
using Coda.RoundRobin.Infrastructure.RoundRobin.Constants;
using Exception = System.Exception;

public sealed class EmptyResponseException : InfrastructureException
{
    public override string Code => ErrorCodes.EMPTY_RESPONSE;

    public EmptyResponseException(Uri endpoint, Exception? innerException = null)
        : base($"Endpoint {endpoint} return empty response", innerException)
    {
    }
}
