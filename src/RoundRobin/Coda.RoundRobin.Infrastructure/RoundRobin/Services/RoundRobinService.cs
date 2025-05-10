namespace Coda.RoundRobin.Infrastructure.RoundRobin.Services;

using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using Coda.RoundRobin.Infrastructure.RoundRobin.Exception;
using Coda.RoundRobin.Infrastructure.RoundRobin.Interfaces;
using Microsoft.Extensions.Options;

internal sealed class RoundRobinService : IRoundRobinService
{
    private const string POST_ENDPOINT = "/api/post";

    private readonly IEndpointResolver endpointResolver;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly RoundRobinOptions options;

    public RoundRobinService(IEndpointResolver endpointResolver, IHttpClientFactory httpClientFactory, IOptions<RoundRobinOptions> options)
    {
        this.endpointResolver = endpointResolver;
        this.httpClientFactory = httpClientFactory;
        this.options = options.Value;
    }

    public IReadOnlyList<Uri> GetEndpoints()
        => this.options.Endpoints;

    public async Task<JsonObject> PostAsync(JsonObject request, CancellationToken cancellationToken)
    {
        var endpoint = this.endpointResolver.GetNextEndpoint();

        try
        {
            using var httpClient = this.httpClientFactory.CreateClient();

            httpClient.BaseAddress = endpoint;

            var response = await httpClient.PostAsJsonAsync(POST_ENDPOINT, request, cancellationToken);

            if (response.IsSuccessStatusCode is false)
            {
                throw new InvalidResponseCodeException(endpoint, response.StatusCode);
            }

            var result = await response.Content.ReadFromJsonAsync<JsonObject>(cancellationToken);

            if (result is null)
            {
                throw new EmptyResponseException(endpoint);
            }

            return result;
        }
        catch (JsonException exception)
        {
            throw new InvalidResponseException(endpoint, exception);
        }
        catch (HttpRequestException exception)
        {
            throw new InvalidResponseCodeException(endpoint, exception.StatusCode?.ToString() ?? @"n\a", exception);
        }
    }
}
