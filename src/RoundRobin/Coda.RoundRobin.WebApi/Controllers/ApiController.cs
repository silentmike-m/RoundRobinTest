namespace Coda.RoundRobin.WebApi.Controllers;

using System.Text.Json.Nodes;
using Coda.RoundRobin.Infrastructure.RoundRobin.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController, Route("api")]
public sealed class ApiController : ControllerBase
{
    private readonly ILogger<ApiController> logger;
    private readonly IRoundRobinService roundRobinService;

    public ApiController(ILogger<ApiController> logger, IRoundRobinService roundRobinService)
    {
        this.logger = logger;
        this.roundRobinService = roundRobinService;
    }

    [Route("get"), HttpGet, ProducesResponseType(typeof(IReadOnlyList<Uri>), StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        this.logger.LogInformation("Received get request");

        var result = this.roundRobinService.GetEndpoints();

        return this.Ok(result);
    }

    [Route("post"), HttpPost, ProducesResponseType(typeof(JsonObject), StatusCodes.Status200OK), ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] JsonObject value, CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Received post request");

        var result = await this.roundRobinService.PostAsync(value, cancellationToken);

        return this.Ok(result);
    }
}
