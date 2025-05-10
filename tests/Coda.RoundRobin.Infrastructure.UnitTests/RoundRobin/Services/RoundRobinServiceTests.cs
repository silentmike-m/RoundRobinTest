namespace Coda.RoundRobin.Infrastructure.UnitTests.RoundRobin.Services;

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using Coda.RoundRobin.Infrastructure.RoundRobin;
using Coda.RoundRobin.Infrastructure.RoundRobin.Exception;
using Coda.RoundRobin.Infrastructure.RoundRobin.Interfaces;
using Coda.RoundRobin.Infrastructure.RoundRobin.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;

[TestClass]
public sealed class RoundRobinServiceTests
{
    private static readonly Uri ENDPOINT = new("http://test.domain.com");

    private readonly Mock<IEndpointResolver> endpointResolverMock = new();
    private readonly RoundRobinOptions options = new();
    private Mock<HttpMessageHandler> handlerMock = null!;
    private Mock<IHttpClientFactory> httpClientFactoryMock = null!;
    private RoundRobinService roundRobinService = null!;

    public RoundRobinServiceTests()
    {
        this.endpointResolverMock
            .Setup(service => service.GetNextEndpoint())
            .Returns(ENDPOINT);
    }

    [TestInitialize]
    public void Initialize()
    {
        this.httpClientFactoryMock = new Mock<IHttpClientFactory>();

        this.handlerMock = new Mock<HttpMessageHandler>();

        this.roundRobinService = new RoundRobinService(this.endpointResolverMock.Object, this.httpClientFactoryMock.Object, Options.Create(this.options));
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

        // Act
        var response = await this.roundRobinService.PostAsync(JsonNode.Parse(requestJson)!.AsObject(), CancellationToken.None);

        // Assert
        httpClient.BaseAddress.Should()
            .Be(ENDPOINT);

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
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(m => m.Method == HttpMethod.Post), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = JsonContent.Create(request),
            });

        // Act
        var action = async () => await this.roundRobinService.PostAsync(JsonNode.Parse(requestJson)!.AsObject(), CancellationToken.None);

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

        // Act
        var action = async () => await this.roundRobinService.PostAsync(JsonNode.Parse(requestJson)!.AsObject(), CancellationToken.None);

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

        // Act
        var action = async () => await this.roundRobinService.PostAsync(JsonNode.Parse(requestJson)!.AsObject(), CancellationToken.None);

        // Assert
        await action.Should()
            .ThrowAsync<InvalidResponseException>();
    }
}
