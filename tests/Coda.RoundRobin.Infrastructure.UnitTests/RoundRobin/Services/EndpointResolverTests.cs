namespace Coda.RoundRobin.Infrastructure.UnitTests.RoundRobin.Services;

using Coda.RoundRobin.Infrastructure.RoundRobin;
using Coda.RoundRobin.Infrastructure.RoundRobin.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;

[TestClass]
public sealed class EndpointResolverTests
{
    [TestMethod]
    public void GetNextEndpoint_Should_ReturnEndpointsInProperOrder()
    {
        // Arrange
        var roundRobinOptions = new RoundRobinOptions
        {
            Endpoints =
            [
                new Uri("http://domain-01"),
                new Uri("http://domain-02"),
                new Uri("http://domain-03"),
            ],
        };

        var service = new EndpointResolver(Options.Create(roundRobinOptions));

        // Act
        var endpoint01 = service.GetNextEndpoint();
        var endpoint02 = service.GetNextEndpoint();
        var endpoint03 = service.GetNextEndpoint();
        var endpoint04 = service.GetNextEndpoint();

        // Assert
        endpoint01.Should()
            .Be(roundRobinOptions.Endpoints[0]);

        endpoint02.Should()
            .Be(roundRobinOptions.Endpoints[1]);

        endpoint03.Should()
            .Be(roundRobinOptions.Endpoints[2]);

        endpoint04.Should()
            .Be(roundRobinOptions.Endpoints[0]);
    }
}
