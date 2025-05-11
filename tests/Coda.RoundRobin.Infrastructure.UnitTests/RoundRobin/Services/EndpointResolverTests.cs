namespace Coda.RoundRobin.Infrastructure.UnitTests.RoundRobin.Services;

using Coda.RoundRobin.Infrastructure.Cache.Interfaces;
using Coda.RoundRobin.Infrastructure.Cache.Models;
using Coda.RoundRobin.Infrastructure.RoundRobin;
using Coda.RoundRobin.Infrastructure.RoundRobin.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;

[TestClass]
public sealed class EndpointResolverTests
{
    [TestMethod]
    public async Task GetNextEndpoint_Should_ReturnEndpointsInProperOrder()
    {
        // Arrange
        var index = 0;

        var roundRobinOptions = new RoundRobinOptions
        {
            Endpoints =
            [
                new Uri("http://domain-01"),
                new Uri("http://domain-02"),
                new Uri("http://domain-03"),
            ],
        };

        var cacheService = new Mock<ICacheService>();

        cacheService
            .Setup(service => service.GetAsync(It.IsAny<CurrentEndpointIndexCacheKey>(), It.IsAny<CancellationToken>()))
            .Returns(() => Task.FromResult(index));

        cacheService
            .Setup(service => service.SetAsync(It.IsAny<CacheKey<int>>(), It.IsAny<int>(), null, It.IsAny<CancellationToken>()))
            .Callback<CacheKey<int>, int, TimeSpan, CancellationToken>((_, currentIndex, _, _) => index = currentIndex);

        var service = new EndpointResolver(cacheService.Object, Options.Create(roundRobinOptions));

        // Act
        var endpoint01 = await service.GetNextEndpointAsync(CancellationToken.None);
        var endpoint02 = await service.GetNextEndpointAsync(CancellationToken.None);
        var endpoint03 = await service.GetNextEndpointAsync(CancellationToken.None);
        var endpoint04 = await service.GetNextEndpointAsync(CancellationToken.None);

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
