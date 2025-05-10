namespace Coda.RoundRobin.WebApi.Filters;

using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using Coda.RoundRobin.Infrastructure.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

[ExcludeFromCodeCoverage]
internal sealed class ApiExceptionFilterAttribute : ExceptionFilterAttribute
{
    private readonly ILogger<ApiExceptionFilterAttribute> logger;

    public ApiExceptionFilterAttribute(ILogger<ApiExceptionFilterAttribute> logger)
        => this.logger = logger;

    public override void OnException(ExceptionContext context)
    {
        this.logger.LogError(context.Exception, "{Message}", context.Exception.Message);

        switch (context.Exception)
        {
            case InfrastructureException applicationException:
                HandleInfrastructureException(context, applicationException);

                break;
            default:
                HandleUnknownException(context);

                break;
        }

        base.OnException(context);
    }

    private static void HandleInfrastructureException(ExceptionContext context, InfrastructureException exception)
    {
        var response = new
        {
            exception.Code,
            Error = exception.Message,
            Response = exception.InnerException?.Message ?? exception.Message,
        };

        context.Result = new ObjectResult(response)
        {
            ContentTypes =
            [
                MediaTypeNames.Application.Json,
            ],
            StatusCode = StatusCodes.Status500InternalServerError,
        };

        context.ExceptionHandled = true;
    }

    private static void HandleUnknownException(ExceptionContext context)
    {
        context.ExceptionHandled = false;
    }
}
