namespace Coda.SimpleWebApi.Controllers;

using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;

[ApiController, Route("api")]
public sealed class ApiController : ControllerBase
{
    private readonly ILogger<ApiController> logger;

    public ApiController(ILogger<ApiController> logger)
        => this.logger = logger;

    [Route("post"), HttpPost, ProducesResponseType(typeof(JsonObject), StatusCodes.Status200OK), ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult Post([FromBody] JsonObject value)
    {
        this.logger.LogInformation("Received post request");

        return this.Ok(value);
    }
}
