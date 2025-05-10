namespace Coda.RoundRobin.Application.UnitTests.RoundRobin.CommandHandlers;

using System.Text.Json;
using System.Text.Json.Nodes;
using Coda.RoundRobin.Application.RoundRobin.CommandHandlers;
using Coda.RoundRobin.Application.RoundRobin.Commands;
using Coda.RoundRobin.Application.RoundRobin.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

[TestClass]
public sealed class PostHandlerTests
{
    private readonly PostHandler handler;
    private readonly NullLogger<PostHandler> logger = new();
    private readonly Mock<IRoundRobinService> roundRobinServiceMock = new();

    public PostHandlerTests()
        => this.handler = new PostHandler(this.logger, this.roundRobinServiceMock.Object);

    [TestMethod]
    public async Task Handle_Should_PostAsync()
    {
        // Assert
        var request = new
        {
            firstName = "John",
            lastName = "Doe",
        };

        var requestJson = JsonSerializer.Serialize(request);

        var postRequest = new Post
        {
            Value = JsonNode.Parse(requestJson)!.AsObject(),
        };

        this.roundRobinServiceMock
            .Setup(service => service.PostAsync(postRequest.Value, It.IsAny<CancellationToken>()))
            .ReturnsAsync(postRequest.Value);

        // Act
        var result = await this.handler.Handle(postRequest, CancellationToken.None);

        // Assert
        result.Should()
            .BeEquivalentTo(postRequest.Value);
    }
}
