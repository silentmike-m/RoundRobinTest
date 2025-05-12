namespace Coda.RoundRobin.Infrastructure.RoundRobin.Exception;

using Coda.RoundRobin.Infrastructure.Common;
using Coda.RoundRobin.Infrastructure.RoundRobin.Constants;
using Exception = System.Exception;

public sealed class HealthyEndpointNotFoundException : InfrastructureException
{
    public override string Code => ErrorCodes.HEALTHY_ENDPOINT_NOT_FOUND;

    public HealthyEndpointNotFoundException(Exception? innerException = null)
        : base($"Healthy endpoint has not been found", innerException)
    {
    }
}
