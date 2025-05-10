namespace Coda.RoundRobin.Infrastructure.RoundRobin.Exception;

using System.Net;
using Coda.RoundRobin.Infrastructure.Common;
using Coda.RoundRobin.Infrastructure.RoundRobin.Constants;
using Exception = System.Exception;

public sealed class InvalidResponseCodeException : InfrastructureException
{
    public override string Code => ErrorCodes.INVALID_RESPONSE_CODE;

    public InvalidResponseCodeException(Uri endpoint, HttpStatusCode responseCode, Exception? innerException = null)
        : base($"Endpoint {endpoint} return invalid response code {responseCode}", innerException)
    {
    }

    public InvalidResponseCodeException(Uri endpoint, string responseCode, Exception? innerException = null)
        : base($"Endpoint {endpoint} return invalid response code {responseCode}", innerException)
    {
    }
}
