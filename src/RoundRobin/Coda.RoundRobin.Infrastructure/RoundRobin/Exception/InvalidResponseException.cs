namespace Coda.RoundRobin.Infrastructure.RoundRobin.Exception;

using Coda.RoundRobin.Infrastructure.Common;
using Coda.RoundRobin.Infrastructure.RoundRobin.Constants;
using Exception = System.Exception;

public sealed class InvalidResponseException : InfrastructureException
{
    public override string Code => ErrorCodes.INVALID_RESPONSE;

    public InvalidResponseException(Uri endpoint, Exception? innerException = null)
        : base($"Endpoint {endpoint} return invalid response", innerException)
    {
    }
}
