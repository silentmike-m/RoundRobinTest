namespace Coda.SimpleWebApi.Controllers;

using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

[ApiController, Route("api")]
public sealed class ApiController : ControllerBase
{
    private readonly ILogger<ApiController> logger;
    private readonly SimpleApiOptions options;

    public ApiController(ILogger<ApiController> logger, IOptions<SimpleApiOptions> options)
    {
        this.logger = logger;
        this.options = options.Value;
    }

    [Route("post"), HttpPost, ProducesResponseType(typeof(JsonObject), StatusCodes.Status200OK), ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] JsonObject value)
    {
        this.logger.LogInformation("Received post request");

        if (this.options.ThrowExceptions)
        {
            throw new Exception("Something went wrong");
        }

        if (this.options.WaitTimeInSeconds > 0)
        {
            await Task.Delay(TimeSpan.FromSeconds(this.options.WaitTimeInSeconds));
        }

        return this.Ok(value);
    }
}
