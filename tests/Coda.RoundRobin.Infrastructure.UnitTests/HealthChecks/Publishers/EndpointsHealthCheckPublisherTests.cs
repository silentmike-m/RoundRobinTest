namespace Coda.RoundRobin.Infrastructure.UnitTests.HealthChecks.Publishers;

using Coda.RoundRobin.Infrastructure.HealthChecks.Publishers;
using Coda.RoundRobin.Infrastructure.RoundRobin.Enums;
using Coda.RoundRobin.Infrastructure.RoundRobin.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;

[TestClass]
public sealed class EndpointsHealthCheckPublisherTests
{
    [TestMethod]
    public async Task PublishAsync_Should_UpdateEndpoints()
    {
        // Arrange
        IReadOnlyDictionary<string, EndpointStatus> endpointStatus = null;

        const string degradatedApiName = "api-01";
        const string healthyApiName = "api-02";
        const string unhealthyApiName = "api-03";

        var report = new HealthReport(new Dictionary<string, HealthReportEntry>
        {
            { degradatedApiName, new HealthReportEntry(HealthStatus.Degraded, duration: TimeSpan.MaxValue, description: null, exception: null, data: null) },
            { healthyApiName, new HealthReportEntry(HealthStatus.Healthy, duration: TimeSpan.MaxValue, description: null, exception: null, data: null) },
            { unhealthyApiName, new HealthReportEntry(HealthStatus.Unhealthy, duration: TimeSpan.MaxValue, description: null, exception: null, data: null) },
        }, HealthStatus.Degraded, TimeSpan.MaxValue);

        var endpointResolverMock = new Mock<IEndpointResolver>();

        endpointResolverMock
            .Setup(service => service.UpdateEndpointsAsync(It.IsAny<IReadOnlyDictionary<string, EndpointStatus>>(), It.IsAny<CancellationToken>()))
            .Callback<IReadOnlyDictionary<string, EndpointStatus>, CancellationToken>((updatedEndpoints, _) => endpointStatus = updatedEndpoints);

        var publisher = new EndpointsHealthCheckPublisher(endpointResolverMock.Object);

        // Act
        await publisher.PublishAsync(report, CancellationToken.None);

        // Assert
        var expectedEndpoints = new Dictionary<string, EndpointStatus>
        {
            { degradatedApiName, EndpointStatus.Unhealthy },
            { healthyApiName, EndpointStatus.Healthy },
            { unhealthyApiName, EndpointStatus.Unhealthy },
        };

        endpointStatus.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo(expectedEndpoints);
    }
}
