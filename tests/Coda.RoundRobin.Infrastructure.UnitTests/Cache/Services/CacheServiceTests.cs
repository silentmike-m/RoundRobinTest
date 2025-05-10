namespace Coda.RoundRobin.Infrastructure.UnitTests.Cache.Services;

using Coda.RoundRobin.Infrastructure.Cache.Models;
using Coda.RoundRobin.Infrastructure.Cache.Services;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

[TestClass]
public sealed class CacheServiceTests
{
    private Mock<IDistributedCache> cacheMock = null!;
    private CacheService cacheService = null!;
    private NullLogger<CacheService> logger = null!;

    [TestMethod]
    public async Task ClearAsync_DoesNotThrow_When_CacheErrorOccurs()
    {
        // Arrange
        this.cacheMock
            .Setup(c => c.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Cache clear error"));

        var cacheKey = new CurrentEndpointIndexCacheKey();

        // Act
        var action = async () => await this.cacheService.ClearAsync(cacheKey, CancellationToken.None);

        // Assert
        await action.Should()
            .NotThrowAsync();
    }

    [TestMethod]
    public async Task GetAsync_ReturnsDefault_WhenCacheErrorOccurs()
    {
        // Arrange
        var cacheKey = new CurrentEndpointIndexCacheKey();

        this.cacheMock
            .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Cache get error"));

        // Act
        var result = await this.cacheService.GetAsync(cacheKey, CancellationToken.None);

        // Assert
        result.Should()
            .Be(0);
    }

    [TestMethod]
    public async Task SetAsync_DoesNotThrow_WhenCacheErrorOccurs()
    {
        // Arrange
        this.cacheMock
            .Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Cache set error"));

        var cacheKey = new CurrentEndpointIndexCacheKey();

        // Act
        var action = async () => await this.cacheService.SetAsync(cacheKey, value: 2, keyTimeoutInMinutes: 5, CancellationToken.None);

        // Assert
        await action.Should()
            .NotThrowAsync();
    }

    [TestInitialize]
    public void Setup()
    {
        this.cacheMock = new Mock<IDistributedCache>();
        this.logger = new NullLogger<CacheService>();
        this.cacheService = new CacheService(this.cacheMock.Object, this.logger);
    }
}
