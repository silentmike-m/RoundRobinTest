namespace Coda.RoundRobin.WebApi.Filters;

using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using Coda.RoundRobin.Application.Common;
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
            case ValidationException validationException:
                HandleValidationException(context, validationException);

                break;

            case InfrastructureException infrastructureException:
                HandleInfrastructureException(context, infrastructureException);

                break;
            case ApplicationException applicationException:
                HandleApplicationException(context, applicationException);

                break;
            default:
                HandleUnknownException(context);

                break;
        }

        base.OnException(context);
    }

    private static void HandleApplicationException(ExceptionContext context, ApplicationException exception)
        => HandleException(context, exception.Code, exception, StatusCodes.Status500InternalServerError);

    private static void HandleException(ExceptionContext context, string code, Exception exception, int statusCode)
    {
        var response = new
        {
            code,
            Error = exception.Message,
            Response = exception.InnerException?.Message ?? exception.Message,
        };

        context.Result = new ObjectResult(response)
        {
            ContentTypes =
            [
                MediaTypeNames.Application.Json,
            ],
            StatusCode = statusCode,
        };

        context.ExceptionHandled = true;
    }

    private static void HandleInfrastructureException(ExceptionContext context, InfrastructureException exception)
        => HandleException(context, exception.Code, exception, StatusCodes.Status500InternalServerError);

    private static void HandleUnknownException(ExceptionContext context)
    {
        context.ExceptionHandled = false;
    }

    private static void HandleValidationException(ExceptionContext context, ValidationException exception)
    {
        {
            var response = new
            {
                exception.Code,
                Error = exception.Message,
                Response = exception.Errors,
            };

            context.Result = new ObjectResult(response)
            {
                ContentTypes =
                [
                    MediaTypeNames.Application.Json,
                ],
                StatusCode = StatusCodes.Status400BadRequest,
            };

            context.ExceptionHandled = true;
        }
    }
}
