namespace Coda.SimpleWebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[ApiController]
public sealed class HomeController : ControllerBase
{
    private readonly ILogger<HomeController> logger;

    public HomeController(ILogger<HomeController> logger)
        => this.logger = logger;

    [Route(""), HttpGet, ApiExplorerSettings(IgnoreApi = true)]
    public RedirectResult Index()
    {
        this.logger.LogInformation("Redirect to Swagger");

        return this.Redirect("swagger/index.html");
    }
}
