namespace Coda.RoundRobin.Infrastructure.UnitTests.RoundRobin.QueryHandlers;

using Coda.RoundRobin.Application.RoundRobin.Dto;
using Coda.RoundRobin.Application.RoundRobin.Queries;
using Coda.RoundRobin.Infrastructure.RoundRobin.Enums;
using Coda.RoundRobin.Infrastructure.RoundRobin.Interfaces;
using Coda.RoundRobin.Infrastructure.RoundRobin.Models;
using Coda.RoundRobin.Infrastructure.RoundRobin.QueryHandlers;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

[TestClass]
public sealed class GetEndpointsHandlerTests
{
    private const string ENDPOINT_01_NAME = "endpoint-01";
    private const EndpointStatus ENDPOINT_01_STATUS = EndpointStatus.Healthy;
    private const string ENDPOINT_02_NAME = "endpoint-02";
    private const EndpointStatus ENDPOINT_02_STATUS = EndpointStatus.Unhealthy;
    private static readonly Uri ENDPOINT_01_URI = new("http://01.domain.com");
    private static readonly Uri ENDPOINT_02_URI = new("http://02.domain.com");
    private readonly Mock<IEndpointResolver> endpointResolverMock = new();

    private readonly NullLogger<GetEndpointsHandler> logger = new();

    public GetEndpointsHandlerTests()
    {
        this.endpointResolverMock
            .Setup(service => service.GetEndpointsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new Endpoint
                {
                    Name = ENDPOINT_01_NAME,
                    Status = ENDPOINT_01_STATUS,
                    Uri = ENDPOINT_01_URI,
                },
                new Endpoint
                {
                    Name = ENDPOINT_02_NAME,
                    Status = ENDPOINT_02_STATUS,
                    Uri = ENDPOINT_02_URI,
                },
            ]);
    }

    [TestMethod]
    public async Task Handle_Should_ReturnEnpoints()
    {
        // Arrange
        var request = new GetEndpoints();

        var handler = new GetEndpointsHandler(this.endpointResolverMock.Object, this.logger);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        var expectedResult = new List<EndpointDto>
        {
            new()
            {
                Name = ENDPOINT_01_NAME,
                Status = ENDPOINT_01_STATUS.ToString(),
                Uri = ENDPOINT_01_URI,
            },
            new()
            {
                Name = ENDPOINT_02_NAME,
                Status = ENDPOINT_02_STATUS.ToString(),
                Uri = ENDPOINT_02_URI,
            },
        };

        result.Should()
            .BeEquivalentTo(expectedResult);
    }
}
