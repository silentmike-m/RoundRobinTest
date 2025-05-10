namespace Coda.RoundRobin.Infrastructure.UnitTests.RoundRobin.Services;

using System.Net;
using Coda.RoundRobin.Infrastructure.RoundRobin;
using Coda.RoundRobin.Infrastructure.RoundRobin.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

[TestClass]
public sealed class RetryPolicyFactoryTests
{
    private readonly NullLogger<RetryPolicyFactory> logger = new();

    [TestMethod]
    public async Task ExecutePolicy_Should_CancelRetry_When_Timeout()
    {
        // Arrange
        var requestCount = 0;
        const int expectedRequestCount = 1;

        const HttpStatusCode excludedStatusCode = HttpStatusCode.BadRequest;

        var options = Options.Create(new RoundRobinOptions
        {
            MaxRetries = 10,
            RetryDelayInSeconds = [5],
            RetryTimeoutInSeconds = 2,
        });

        var policyFactory = new RetryPolicyFactory(options);

        var policy = policyFactory.CreateRetryPolicy(this.logger, [excludedStatusCode]);

        // Act
        await policy.ExecuteAsync(async _ =>
        {
            requestCount++;

            return await Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        });

        // Assert
        requestCount.Should()
            .Be(expectedRequestCount);
    }

    [TestMethod]
    public async Task ExecutePolicy_Should_NotRetry_When_HttpResponseIsOk()
    {
        // Arrange
        var requestCount = 0;
        const int expectedRequestCount = 1;

        const HttpStatusCode excludedStatusCode = HttpStatusCode.BadRequest;

        var options = Options.Create(new RoundRobinOptions
        {
            MaxRetries = 5,
            RetryDelayInSeconds = [1],
            RetryTimeoutInSeconds = 5,
        });

        var policyFactory = new RetryPolicyFactory(options);

        var policy = policyFactory.CreateRetryPolicy(this.logger, [excludedStatusCode]);

        // Act
        await policy.ExecuteAsync(async _ =>
        {
            requestCount++;

            return await Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        });

        // Assert
        requestCount.Should()
            .Be(expectedRequestCount);
    }

    [TestMethod]
    public async Task ExecutePolicy_Should_NotRetry_When_HttpResponseMessageCodeIsExcluded()
    {
        // Arrange
        var requestCount = 0;
        const int expectedRequestCount = 1;

        const HttpStatusCode excludedStatusCode = HttpStatusCode.BadRequest;

        var options = Options.Create(new RoundRobinOptions
        {
            MaxRetries = 5,
            RetryDelayInSeconds = [1],
            RetryTimeoutInSeconds = 5,
        });

        var policyFactory = new RetryPolicyFactory(options);

        var policy = policyFactory.CreateRetryPolicy(this.logger, [excludedStatusCode]);

        // Act
        await policy.ExecuteAsync(async _ =>
        {
            requestCount++;

            return await Task.FromResult(new HttpResponseMessage(excludedStatusCode));
        });

        // Assert
        requestCount.Should()
            .Be(expectedRequestCount);
    }

    [DataTestMethod, DataRow(HttpStatusCode.InternalServerError), DataRow(HttpStatusCode.BadGateway)]
    public async Task ExecutePolicy_Should_Retry_When_HttpResponseMessageCodeIsNotExcluded(HttpStatusCode statusCode)
    {
        // Arrange
        var requestCount = 0;
        const int expectedRequestCount = 4;

        var options = Options.Create(new RoundRobinOptions
        {
            MaxRetries = 3,
            RetryDelayInSeconds = [1],
            RetryTimeoutInSeconds = 10,
        });

        var policyFactory = new RetryPolicyFactory(options);

        var policy = policyFactory.CreateRetryPolicy(this.logger);

        // Act
        await policy.ExecuteAsync(async _ =>
        {
            requestCount++;

            return await Task.FromResult(new HttpResponseMessage(statusCode));
        });

        // Assert
        requestCount.Should()
            .Be(expectedRequestCount);
    }
}
