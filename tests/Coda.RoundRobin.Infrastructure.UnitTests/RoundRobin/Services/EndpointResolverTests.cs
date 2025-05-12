namespace Coda.RoundRobin.Infrastructure.UnitTests.RoundRobin.Services;

using Coda.RoundRobin.Infrastructure.Cache.Interfaces;
using Coda.RoundRobin.Infrastructure.Cache.Models;
using Coda.RoundRobin.Infrastructure.RoundRobin;
using Coda.RoundRobin.Infrastructure.RoundRobin.Enums;
using Coda.RoundRobin.Infrastructure.RoundRobin.Exception;
using Coda.RoundRobin.Infrastructure.RoundRobin.Models;
using Coda.RoundRobin.Infrastructure.RoundRobin.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;

[TestClass]
public sealed class EndpointResolverTests
{
    [TestMethod]
    public async Task GetEndpointsAsync_Should_ReturnEmptyList_When_MissingEndpoints()
    {
        // Arrange
        var cacheServiceMock = new Mock<ICacheService>();
        var options = Options.Create(new RoundRobinOptions());

        var endpointResolver = new EndpointResolver(cacheServiceMock.Object, options);

        // Act
        var result = await endpointResolver.GetEndpointsAsync(CancellationToken.None);

        // Assert
        cacheServiceMock
            .Verify(service => service.GetAsync(It.IsAny<EndpointsCacheKey>(), It.IsAny<CancellationToken>()), Times.Once);

        result.Should()
            .BeEmpty();
    }

    [TestMethod]
    public async Task GetEndpointsAsync_Should_ReturnEndpoints()
    {
        // Arrange
        var endpoints = new List<Endpoint>
        {
            new()
            {
                Uri = new Uri("http://01.domain.com"),
                Name = "01",
                Status = EndpointStatus.Healthy,
            },
            new()
            {
                Uri = new Uri("http://02.domain.com"),
                Name = "02",
                Status = EndpointStatus.Unhealthy,
            },
            new()
            {
                Uri = new Uri("http://03.domain.com"),
                Name = "03",
                Status = EndpointStatus.Slow,
            },
        };

        var cacheServiceMock = new Mock<ICacheService>();
        var options = Options.Create(new RoundRobinOptions());

        cacheServiceMock
            .Setup(service => service.GetAsync(It.IsAny<EndpointsCacheKey>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(endpoints);

        var endpointResolver = new EndpointResolver(cacheServiceMock.Object, options);

        // Act
        var result = await endpointResolver.GetEndpointsAsync(CancellationToken.None);

        // Assert
        cacheServiceMock
            .Verify(service => service.GetAsync(It.IsAny<EndpointsCacheKey>(), It.IsAny<CancellationToken>()), Times.Once);

        result.Should()
            .BeEquivalentTo(endpoints);
    }

    [TestMethod]
    public async Task GetNextEndpointAsync_Should_ReturnNextWorkingEndpoint()
    {
        // Arrange
        var endpoints = new List<Endpoint>
        {
            new()
            {
                Uri = new Uri("http://01.domain.com"),
                Name = "01",
                Status = EndpointStatus.Healthy,
            },
            new()
            {
                Uri = new Uri("http://02.domain.com"),
                Name = "02",
                Status = EndpointStatus.Unhealthy,
            },
            new()
            {
                Uri = new Uri("http://03.domain.com"),
                Name = "03",
                Status = EndpointStatus.Slow,
            },
        };

        var cacheServiceMock = new Mock<ICacheService>();
        var options = Options.Create(new RoundRobinOptions());

        cacheServiceMock
            .Setup(service => service.GetAsync(It.IsAny<CurrentEndpointIndexCacheKey>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        cacheServiceMock
            .Setup(service => service.GetAsync(It.IsAny<EndpointsCacheKey>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(endpoints);

        var endpointResolver = new EndpointResolver(cacheServiceMock.Object, options);

        // Act
        var result = await endpointResolver.GetNextEndpointAsync(CancellationToken.None);

        // Assert
        cacheServiceMock
            .Verify(service => service.GetAsync(It.IsAny<CurrentEndpointIndexCacheKey>(), It.IsAny<CancellationToken>()), Times.Once);

        cacheServiceMock
            .Verify(service => service.GetAsync(It.IsAny<EndpointsCacheKey>(), It.IsAny<CancellationToken>()), Times.Once);

        cacheServiceMock
            .Verify(service => service.SetAsync(It.IsAny<CurrentEndpointIndexCacheKey>(), 0, null, It.IsAny<CancellationToken>()));

        result.Should()
            .Be(endpoints[0].Uri);
    }

    [TestMethod]
    public async Task GetNextEndpointAsync_Should_ReturnNextWorkingEndpoint2()
    {
        // Arrange
        var endpoints = new List<Endpoint>
        {
            new()
            {
                Uri = new Uri("http://01.domain.com"),
                Name = "01",
                Status = EndpointStatus.Unhealthy,
            },
            new()
            {
                Uri = new Uri("http://02.domain.com"),
                Name = "02",
                Status = EndpointStatus.Unhealthy,
            },
            new()
            {
                Uri = new Uri("http://03.domain.com"),
                Name = "03",
                Status = EndpointStatus.Healthy,
            },
        };

        var cacheServiceMock = new Mock<ICacheService>();
        var options = Options.Create(new RoundRobinOptions());

        cacheServiceMock
            .Setup(service => service.GetAsync(It.IsAny<CurrentEndpointIndexCacheKey>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(3);

        cacheServiceMock
            .Setup(service => service.GetAsync(It.IsAny<EndpointsCacheKey>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(endpoints);

        var endpointResolver = new EndpointResolver(cacheServiceMock.Object, options);

        // Act
        var result = await endpointResolver.GetNextEndpointAsync(CancellationToken.None);

        // Assert
        cacheServiceMock
            .Verify(service => service.GetAsync(It.IsAny<CurrentEndpointIndexCacheKey>(), It.IsAny<CancellationToken>()), Times.Once);

        cacheServiceMock
            .Verify(service => service.GetAsync(It.IsAny<EndpointsCacheKey>(), It.IsAny<CancellationToken>()), Times.Once);

        cacheServiceMock
            .Verify(service => service.SetAsync(It.IsAny<CurrentEndpointIndexCacheKey>(), 2, null, It.IsAny<CancellationToken>()));

        result.Should()
            .Be(endpoints[2].Uri);
    }

    [TestMethod]
    public async Task GetNextEndpointAsync_Should_ReturnNextWorkingEndpoint3()
    {
        // Arrange
        var endpoints = new List<Endpoint>
        {
            new()
            {
                Uri = new Uri("http://01.domain.com"),
                Name = "01",
                Status = EndpointStatus.Healthy,
            },
            new()
            {
                Uri = new Uri("http://02.domain.com"),
                Name = "02",
                Status = EndpointStatus.Healthy,
            },
            new()
            {
                Uri = new Uri("http://03.domain.com"),
                Name = "03",
                Status = EndpointStatus.Unhealthy,
            },
        };

        var cacheServiceMock = new Mock<ICacheService>();
        var options = Options.Create(new RoundRobinOptions());

        cacheServiceMock
            .Setup(service => service.GetAsync(It.IsAny<CurrentEndpointIndexCacheKey>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        cacheServiceMock
            .Setup(service => service.GetAsync(It.IsAny<EndpointsCacheKey>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(endpoints);

        var endpointResolver = new EndpointResolver(cacheServiceMock.Object, options);

        // Act
        var result = await endpointResolver.GetNextEndpointAsync(CancellationToken.None);

        // Assert
        cacheServiceMock
            .Verify(service => service.GetAsync(It.IsAny<CurrentEndpointIndexCacheKey>(), It.IsAny<CancellationToken>()), Times.Once);

        cacheServiceMock
            .Verify(service => service.GetAsync(It.IsAny<EndpointsCacheKey>(), It.IsAny<CancellationToken>()), Times.Once);

        cacheServiceMock
            .Verify(service => service.SetAsync(It.IsAny<CurrentEndpointIndexCacheKey>(), 0, null, It.IsAny<CancellationToken>()));

        result.Should()
            .Be(endpoints[0].Uri);
    }

    [TestMethod]
    public async Task GetNextEndpointAsync_Should_ThrowHealthyEndpointNotFoundException_When_MissingHealthyEndpoint()
    {
        // Arrange
        var endpoints = new List<Endpoint>
        {
            new()
            {
                Uri = new Uri("http://01.domain.com"),
                Name = "01",
                Status = EndpointStatus.Unhealthy,
            },
            new()
            {
                Uri = new Uri("http://02.domain.com"),
                Name = "02",
                Status = EndpointStatus.Unhealthy,
            },
            new()
            {
                Uri = new Uri("http://03.domain.com"),
                Name = "03",
                Status = EndpointStatus.Slow,
            },
        };

        var cacheServiceMock = new Mock<ICacheService>();
        var options = Options.Create(new RoundRobinOptions());

        cacheServiceMock
            .Setup(service => service.GetAsync(It.IsAny<CurrentEndpointIndexCacheKey>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        cacheServiceMock
            .Setup(service => service.GetAsync(It.IsAny<EndpointsCacheKey>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(endpoints);

        var endpointResolver = new EndpointResolver(cacheServiceMock.Object, options);

        // Act
        var action = async () => await endpointResolver.GetNextEndpointAsync(CancellationToken.None);

        // Assert
        await action.Should()
            .ThrowAsync<HealthyEndpointNotFoundException>();
    }

    [TestMethod]
    public async Task InitializeEndpointsAsync_Should_InitializeEndpoints()
    {
        // Arrange
        var endpoints = new List<Endpoint>();

        var optionsEndpoints = new Dictionary<string, Uri>
        {
            { "01", new Uri("http://01.domain.com") },
            { "02", new Uri("http://02.domain.com") },
        };

        var cacheServiceMock = new Mock<ICacheService>();

        var options = Options.Create(new RoundRobinOptions
        {
            Endpoints = optionsEndpoints,
        });

        cacheServiceMock
            .Setup(service => service.SetAsync(It.IsAny<CacheKey<IReadOnlyList<Endpoint>>>(), It.IsAny<IReadOnlyList<Endpoint>>(), null, It.IsAny<CancellationToken>()))
            .Callback<CacheKey<IReadOnlyList<Endpoint>>, IReadOnlyList<Endpoint>, int?, CancellationToken>((_, savedEndpoints, _, _) => endpoints.AddRange(savedEndpoints));

        var endpointResolver = new EndpointResolver(cacheServiceMock.Object, options);

        // Act
        await endpointResolver.InitializeEndpointsAsync(CancellationToken.None);

        // Assert
        var expectedEndpoints = new List<Endpoint>
        {
            new()
            {
                Uri = new Uri("http://01.domain.com"),
                Name = "01",
                Status = EndpointStatus.Healthy,
            },
            new()
            {
                Uri = new Uri("http://02.domain.com"),
                Name = "02",
                Status = EndpointStatus.Healthy,
            },
        };

        endpoints.Should()
            .BeEquivalentTo(expectedEndpoints);
    }

    [TestMethod]
    public async Task UpdateEndpointsAsync_Should_UpdateEndpoints()
    {
        // Arrange
        var endpoints = new List<Endpoint>
        {
            new()
            {
                Uri = new Uri("http://01.domain.com"),
                Name = "01",
                Status = EndpointStatus.Unhealthy,
            },
            new()
            {
                Uri = new Uri("http://02.domain.com"),
                Name = "02",
                Status = EndpointStatus.Unhealthy,
            },
            new()
            {
                Uri = new Uri("http://03.domain.com"),
                Name = "03",
                Status = EndpointStatus.Slow,
            },
        };

        var cacheServiceMock = new Mock<ICacheService>();
        var options = Options.Create(new RoundRobinOptions());

        cacheServiceMock
            .Setup(service => service.GetAsync(It.IsAny<EndpointsCacheKey>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(endpoints);

        var endpointResolver = new EndpointResolver(cacheServiceMock.Object, options);

        // Act
        await endpointResolver.UpdateEndpointsAsync(new Dictionary<string, EndpointStatus>
        {
            { endpoints[0].Name, EndpointStatus.Healthy },
            { endpoints[1].Name, EndpointStatus.Slow },
            { endpoints[2].Name, EndpointStatus.Healthy },
        }, CancellationToken.None);

        // Assert
        cacheServiceMock
            .Verify(service => service.SetAsync(It.IsAny<EndpointsCacheKey>(), endpoints, null, It.IsAny<CancellationToken>()), Times.Once);

        endpoints[0].Status.Should()
            .Be(EndpointStatus.Healthy);

        endpoints[1].Status.Should()
            .Be(EndpointStatus.Slow);

        endpoints[2].Status.Should()
            .Be(EndpointStatus.Healthy);
    }
}
