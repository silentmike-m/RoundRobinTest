namespace Coda.RoundRobin.Infrastructure.RoundRobin.Services;

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using Coda.RoundRobin.Application.RoundRobin.Interfaces;
using Coda.RoundRobin.Infrastructure.RoundRobin.Enums;
using Coda.RoundRobin.Infrastructure.RoundRobin.Exception;
using Coda.RoundRobin.Infrastructure.RoundRobin.Interfaces;
using Microsoft.Extensions.Logging;

internal sealed class RoundRobinService : IRoundRobinService
{
    private const string POST_ENDPOINT = "/api/post";
    private static readonly IReadOnlyList<HttpStatusCode> RETRY_POLICY_EXCLUDED_ERRORS = [HttpStatusCode.BadRequest, HttpStatusCode.Unauthorized];

    private readonly IEndpointResolver endpointResolver;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ILogger<RoundRobinService> logger;
    private readonly IRetryPolicyFactory retryPolicyFactory;

    public RoundRobinService(IEndpointResolver endpointResolver, IHttpClientFactory httpClientFactory, ILogger<RoundRobinService> logger, IRetryPolicyFactory retryPolicyFactory)
    {
        this.endpointResolver = endpointResolver;
        this.httpClientFactory = httpClientFactory;
        this.logger = logger;
        this.retryPolicyFactory = retryPolicyFactory;
    }

    public async Task<JsonObject> PostAsync(JsonObject request, CancellationToken cancellationToken)
    {
        var endpoint = await this.endpointResolver.GetNextEndpointAsync(cancellationToken);

        try
        {
            var retryPolicy = this.retryPolicyFactory.CreateRetryPolicy(this.logger, RETRY_POLICY_EXCLUDED_ERRORS);

            using var httpClient = this.httpClientFactory.CreateClient();

            httpClient.BaseAddress = endpoint.Uri;

            var response = await retryPolicy.ExecuteAsync(async token =>
            {
                using var httpRequest = new HttpRequestMessage();

                httpRequest.RequestUri = new Uri(endpoint.Uri, POST_ENDPOINT);
                httpRequest.Method = HttpMethod.Post;
                httpRequest.Content = JsonContent.Create(request);

                return await httpClient.SendAsync(httpRequest, token);
            }, cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<JsonObject>(cancellationToken);

            if (result is null)
            {
                throw new EmptyResponseException(endpoint.Uri);
            }

            return result;
        }
        catch (JsonException exception)
        {
            throw new InvalidResponseException(endpoint.Uri, exception);
        }
        catch (HttpRequestException exception)
        {
            if (exception.StatusCode >= HttpStatusCode.Unauthorized)
            {
                await this.endpointResolver.UpdateEndpointAsync(endpoint, EndpointStatus.Unhealthy, CancellationToken.None);
            }

            throw new InvalidResponseCodeException(endpoint.Uri, exception.StatusCode?.ToString() ?? @"n\a", exception);
        }
    }
}
