namespace Coda.RoundRobin.Application.RoundRobin.CommandHandlers;

using System.Text.Json.Nodes;
using Coda.RoundRobin.Application.RoundRobin.Commands;
using Coda.RoundRobin.Application.RoundRobin.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

internal sealed class PostHandler : IRequestHandler<Post, JsonObject>
{
    private readonly ILogger<PostHandler> logger;
    private readonly IRoundRobinService roundRobinService;

    public PostHandler(ILogger<PostHandler> logger, IRoundRobinService roundRobinService)
    {
        this.logger = logger;
        this.roundRobinService = roundRobinService;
    }

    public async Task<JsonObject> Handle(Post request, CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Try to send post request");

        return await this.roundRobinService.PostAsync(request.Value, cancellationToken);
    }
}
