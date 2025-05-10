namespace Coda.RoundRobin.Infrastructure.RoundRobin.Interfaces;

using System.Net;
using Microsoft.Extensions.Logging;
using Polly;

internal interface IRetryPolicyFactory
{
    ResiliencePipeline<HttpResponseMessage> CreateRetryPolicy(ILogger logger, IReadOnlyList<HttpStatusCode>? statusCodesExcludedFromRetry = null);
}
