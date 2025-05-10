namespace Coda.RoundRobin.WebApi.Controllers;

using System.Text.Json.Nodes;
using Coda.RoundRobin.Application.RoundRobin.Commands;
using Coda.RoundRobin.Application.RoundRobin.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController, Route("api")]
public sealed class ApiController : ControllerBase
{
    private readonly ILogger<ApiController> logger;
    private readonly ISender mediator;

    public ApiController(ILogger<ApiController> logger, ISender mediator)
    {
        this.logger = logger;
        this.mediator = mediator;
    }

    [Route("get"), HttpGet, ProducesResponseType(typeof(IReadOnlyList<Uri>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync()
    {
        this.logger.LogInformation("Received get request");

        var result = await this.mediator.Send(new GetEndpoints());

        return this.Ok(result);
    }

    [Route("post"), HttpPost, ProducesResponseType(typeof(JsonObject), StatusCodes.Status200OK), ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostAsync([FromBody] JsonObject value, CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Received post request");

        var result = await this.mediator.Send(new Post
        {
            Value = value,
        }, cancellationToken);

        return this.Ok(result);
    }
}
