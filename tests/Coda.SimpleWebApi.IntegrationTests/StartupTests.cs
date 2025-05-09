namespace Coda.SimpleWebApi.IntegrationTests;

using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

[TestClass]
public sealed class StartupTests : IDisposable
{
    private readonly WebApplicationFactory<Program> factory = new();

    public void Dispose()
    {
        this.factory.Dispose();
    }

    [TestMethod]
    public async Task ShouldReturnHealthCheck()
    {
        // Assert
        var client = this.factory.CreateClient();
        const string url = "/hc";

        // Act
        var response = await client.GetAsync(url);

        // Assert
        response.StatusCode.Should()
            .Be(HttpStatusCode.OK)
            ;

        response.Content.Headers.ContentType?.ToString()
            .Should()
            .Be("application/json")
            ;
    }

    [TestMethod]
    public async Task ShouldReturnSwagger()
    {
        // Assert
        var client = this.factory.CreateClient();

        // Act
        var response = await client.GetAsync("/swagger/v1/swagger.json");

        // Assert
        response.StatusCode.Should()
            .Be(HttpStatusCode.OK)
            ;

        response.Content.Headers.ContentType?.ToString()
            .Should()
            .Be("application/json; charset=utf-8")
            ;
    }
}
