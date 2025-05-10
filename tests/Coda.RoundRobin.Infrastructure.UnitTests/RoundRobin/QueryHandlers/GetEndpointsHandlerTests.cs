namespace Coda.RoundRobin.Infrastructure.UnitTests.RoundRobin.QueryHandlers;

using Coda.RoundRobin.Application.RoundRobin.Queries;
using Coda.RoundRobin.Infrastructure.RoundRobin;
using Coda.RoundRobin.Infrastructure.RoundRobin.QueryHandlers;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

[TestClass]
public sealed class GetEndpointsHandlerTests
{
    private readonly NullLogger<GetEndpointsHandler> logger = new();

    private readonly RoundRobinOptions roundRobinOptions = new()
    {
        Endpoints =
        [
            new Uri("http://domain-01.com"),
            new Uri("http://domain-02.com"),
            new Uri("http://domain-03.com"),
        ],
    };

    [TestMethod]
    public async Task Handle_Should_ReturnEnpoints()
    {
        // Arrange
        var request = new GetEndpoints();

        var handler = new GetEndpointsHandler(this.logger, Options.Create(this.roundRobinOptions));

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should()
            .BeEquivalentTo(this.roundRobinOptions.Endpoints);
    }
}
