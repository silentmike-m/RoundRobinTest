namespace Coda.RoundRobin.Infrastructure.RoundRobin.Services;

using System.Net;
using Coda.RoundRobin.Infrastructure.RoundRobin.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using Polly.Timeout;

internal sealed class RetryPolicyFactory : IRetryPolicyFactory
{
    private readonly RoundRobinOptions roundRobinOptions;

    public RetryPolicyFactory(IOptions<RoundRobinOptions> roundRobinOptions)
        => this.roundRobinOptions = roundRobinOptions.Value;

    public ResiliencePipeline<HttpResponseMessage> CreateRetryPolicy(ILogger logger, IReadOnlyList<HttpStatusCode>? statusCodesExcludedFromRetry = null)
    {
        statusCodesExcludedFromRetry ??= new List<HttpStatusCode>();

        var pipelineTimoutOptions = new TimeoutStrategyOptions
        {
            OnTimeout = args =>
            {
                logger.LogInformation("Retry policy timeout after {RetryPolicyTimeout} seconds", args.Timeout.TotalSeconds);

                return default;
            },
            Timeout = TimeSpan.FromSeconds(this.roundRobinOptions.RetryTimeoutInSeconds),
        };

        var pipeline = new ResiliencePipelineBuilder<HttpResponseMessage>()
            .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
            {
                DelayGenerator = args =>
                {
                    var index = args.AttemptNumber;

                    var seconds = this.roundRobinOptions.RetryDelayInSeconds.Length > index
                        ? this.roundRobinOptions.RetryDelayInSeconds[index]
                        : this.roundRobinOptions.RetryDelayInSeconds[^1];

                    return new ValueTask<TimeSpan?>(TimeSpan.FromSeconds(seconds));
                },
                MaxRetryAttempts = this.roundRobinOptions.MaxRetries,
                OnRetry = args =>
                {
                    logger.LogInformation("Retrying sending request for {RetryAttemptNumber} time", args.AttemptNumber + 1);

                    return default;
                },
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .HandleResult(result => HandleResult(result, statusCodesExcludedFromRetry)),
            })
            .AddTimeout(pipelineTimoutOptions)
            .Build();

        return pipeline;
    }

    private static bool HandleResult(HttpResponseMessage response, IReadOnlyList<HttpStatusCode> statusCodesExcludedFromRetry)
    {
        if (statusCodesExcludedFromRetry.Contains(response.StatusCode))
        {
            return false;
        }

        return !response.IsSuccessStatusCode;
    }
}
