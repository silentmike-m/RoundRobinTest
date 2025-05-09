namespace Coda.SimpleWebApi.IntegrationTests;

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

[TestClass]
public sealed class ApiControllerTests : IDisposable
{
    private const string POST_REQUEST_URL = "/api/post";

    private readonly WebApplicationFactory<Program> factory = new();

    public void Dispose()
    {
        this.factory.Dispose();
    }

    [TestMethod]
    public async Task Post_Should_ReturnInternalServerError_When_InvalidJson()
    {
        // Arrange
        var client = this.factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync(POST_REQUEST_URL, "test");

        // Assert
        response.IsSuccessStatusCode.Should()
            .BeFalse();

        response.StatusCode.Should()
            .Be(HttpStatusCode.BadRequest);
    }

    [TestMethod]
    public async Task Post_Should_ReturnRequestJson()
    {
        // Arrange
        var request = new
        {
            firstName = "John",
            lastName = "Doe",
        };

        var client = this.factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync(POST_REQUEST_URL, request);

        // Assert
        response.IsSuccessStatusCode.Should()
            .BeTrue();

        response.StatusCode.Should()
            .Be(HttpStatusCode.OK);

        var expectedJson = JsonSerializer.Serialize(request);

        response.Content.ReadAsStringAsync().Result.Should()
            .Be(expectedJson);
    }
}
