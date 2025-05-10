namespace Coda.RoundRobin.Application.UnitTests.RoundRobin.Validators;

using System.Text.Json;
using System.Text.Json.Nodes;
using Coda.RoundRobin.Application.RoundRobin.Commands;
using Coda.RoundRobin.Application.RoundRobin.Constants;
using Coda.RoundRobin.Application.RoundRobin.Validators;
using FluentAssertions;

[TestClass]
public sealed class PostValidatorTests
{
    private readonly PostValidator validator = new();

    [TestMethod]
    public async Task Validate_Should_Fail_When_JsonIsEmpty()
    {
        var postRequest = new Post
        {
            Value = JsonNode.Parse("{}")!.AsObject(),
        };

        // Act
        var result = await this.validator.ValidateAsync(postRequest);

        // Assert
        result.IsValid.Should()
            .BeFalse();

        result.Errors.Should()
            .HaveCount(1)
            .And
            .Contain(error =>
                error.PropertyName == nameof(Post.Value)
                && error.ErrorCode == ValidationErrorCodes.EMPTY_JSON_REQUEST
                && error.ErrorMessage == ValidationErrorCodes.EMPTY_JSON_REQUEST_MESSAGE
            );
    }

    [TestMethod]
    public async Task Validate_Should_Pass()
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

        // Act
        var result = await this.validator.ValidateAsync(postRequest);

        // Assert
        result.IsValid.Should()
            .BeTrue();

        result.Errors.Should()
            .BeEmpty();
    }
}
