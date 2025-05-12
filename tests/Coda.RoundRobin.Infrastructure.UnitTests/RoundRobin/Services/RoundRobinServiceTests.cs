namespace Coda.RoundRobin.Infrastructure.UnitTests.RoundRobin.Services;

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using Coda.RoundRobin.Infrastructure.RoundRobin;
using Coda.RoundRobin.Infrastructure.RoundRobin.Enums;
using Coda.RoundRobin.Infrastructure.RoundRobin.Exception;
using Coda.RoundRobin.Infrastructure.RoundRobin.Interfaces;
using Coda.RoundRobin.Infrastructure.RoundRobin.Models;
using Coda.RoundRobin.Infrastructure.RoundRobin.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;

[TestClass]
public sealed class RoundRobinServiceTests
{
    private static readonly Endpoint ENDPOINT = new()
    {
        Uri = new Uri("http://test.domain.com"),
        Name = "test-01",
        Status = EndpointStatus.Healthy,
    };

    private readonly Mock<IEndpointResolver> endpointResolverMock = new();
    private readonly NullLogger<RoundRobinService> logger = new();

    private readonly RoundRobinOptions options = new()
    {
        MaxRetries = 1,
        RetryDelayInSeconds = [1],
    };

    private readonly RetryPolicyFactory retryPolicyFactory;
    private Mock<HttpMessageHandler> handlerMock = null!;
    private Mock<IHttpClientFactory> httpClientFactoryMock = null!;

    public RoundRobinServiceTests()
    {
        this.endpointResolverMock
            .Setup(service => service.GetNextEndpointAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(ENDPOINT);

        this.retryPolicyFactory = new RetryPolicyFactory(Options.Create(this.options));
    }

    [TestInitialize]
    public void Initialize()
    {
        this.httpClientFactoryMock = new Mock<IHttpClientFactory>();

        this.handlerMock = new Mock<HttpMessageHandler>();
    }

    [TestMethod]
    public async Task PostAsync_Should_ReturnResponse()
    {
        // Arrange
        var request = new
        {
            firstName = "John",
            lastName = "Doe",
        };

        var requestJson = JsonSerializer.Serialize(request);

        using var httpClient = new HttpClient(this.handlerMock.Object);

        this.httpClientFactoryMock
            .Setup(factory => factory.CreateClient(Options.DefaultName))
            .Returns(httpClient);

        this.handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(m => m.Method == HttpMethod.Post), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(request),
            });

        var roundRobinService = new RoundRobinService(this.endpointResolverMock.Object, this.httpClientFactoryMock.Object, this.logger, this.retryPolicyFactory);

        // Act
        var response = await roundRobinService.PostAsync(JsonNode.Parse(requestJson)!.AsObject(), CancellationToken.None);

        // Assert
        httpClient.BaseAddress.Should()
            .Be(ENDPOINT.Uri);

        response.ToJsonString().Should()
            .Be(requestJson);
    }

    [TestMethod]
    public async Task PostAsync_Should_ReturnResponse_On_SecondTry()
    {
        // Arrange
        var request = new
        {
            firstName = "John",
            lastName = "Doe",
        };

        var requestJson = JsonSerializer.Serialize(request);

        using var httpClient = new HttpClient(this.handlerMock.Object);

        this.httpClientFactoryMock
            .Setup(factory => factory.CreateClient(Options.DefaultName))
            .Returns(httpClient);

        this.handlerMock
            .Protected()
            .SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(m => m.Method == HttpMethod.Post), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(request),
            });

        var roundRobinService = new RoundRobinService(this.endpointResolverMock.Object, this.httpClientFactoryMock.Object, this.logger, this.retryPolicyFactory);

        // Act
        var response = await roundRobinService.PostAsync(JsonNode.Parse(requestJson)!.AsObject(), CancellationToken.None);

        // Assert
        httpClient.BaseAddress.Should()
            .Be(ENDPOINT.Uri);

        response.ToJsonString().Should()
            .Be(requestJson);
    }

    [TestMethod]
    public async Task PostAsync_Should_ThrowInvalidResponseCodeException_When_ResponseCodeIsNotSuccessStatusCode()
    {
        // Arrange
        var request = new
        {
            firstName = "John",
            lastName = "Doe",
        };

        var requestJson = JsonSerializer.Serialize(request);

        using var httpClient = new HttpClient(this.handlerMock.Object);

        this.httpClientFactoryMock
            .Setup(factory => factory.CreateClient(Options.DefaultName))
            .Returns(httpClient);

        this.handlerMock
            .Protected()
            .SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(m => m.Method == HttpMethod.Post), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = JsonContent.Create(request),
            })
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = JsonContent.Create(request),
            });

        var roundRobinService = new RoundRobinService(this.endpointResolverMock.Object, this.httpClientFactoryMock.Object, this.logger, this.retryPolicyFactory);

        // Act
        var action = async () => await roundRobinService.PostAsync(JsonNode.Parse(requestJson)!.AsObject(), CancellationToken.None);

        // Assert
        await action.Should()
            .ThrowAsync<InvalidResponseCodeException>();
    }

    [TestMethod]
    public async Task PostAsync_Should_ThrowInvalidResponseException_When_HttpRequestException()
    {
// Arrange
        var request = new
        {
            firstName = "John",
            lastName = "Doe",
        };

        var requestJson = JsonSerializer.Serialize(request);

        using var httpClient = new HttpClient(this.handlerMock.Object);

        this.httpClientFactoryMock
            .Setup(factory => factory.CreateClient(Options.DefaultName))
            .Returns(httpClient);

        this.handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(m => m.Method == HttpMethod.Post), ItExpr.IsAny<CancellationToken>())
            .Throws<HttpRequestException>();

        var roundRobinService = new RoundRobinService(this.endpointResolverMock.Object, this.httpClientFactoryMock.Object, this.logger, this.retryPolicyFactory);

        // Act
        var action = async () => await roundRobinService.PostAsync(JsonNode.Parse(requestJson)!.AsObject(), CancellationToken.None);

        // Assert
        await action.Should()
            .ThrowAsync<InvalidResponseCodeException>();
    }

    [TestMethod]
    public async Task PostAsync_Should_ThrowInvalidResponseException_When_ResponseIsEmpty()
    {
        // Arrange
        var request = new
        {
            firstName = "John",
            lastName = "Doe",
        };

        var requestJson = JsonSerializer.Serialize(request);

        using var httpClient = new HttpClient(this.handlerMock.Object);

        this.httpClientFactoryMock
            .Setup(factory => factory.CreateClient(Options.DefaultName))
            .Returns(httpClient);

        this.handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(m => m.Method == HttpMethod.Post), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = null,
            });

        var roundRobinService = new RoundRobinService(this.endpointResolverMock.Object, this.httpClientFactoryMock.Object, this.logger, this.retryPolicyFactory);

        // Act
        var action = async () => await roundRobinService.PostAsync(JsonNode.Parse(requestJson)!.AsObject(), CancellationToken.None);

        // Assert
        await action.Should()
            .ThrowAsync<InvalidResponseException>();
    }
}
