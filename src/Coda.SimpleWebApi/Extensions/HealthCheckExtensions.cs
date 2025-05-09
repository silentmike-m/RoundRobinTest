namespace Coda.SimpleWebApi.Extensions;

using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

internal static class HealthCheckExtensions
{
    public static Task WriteResponse(HttpContext context, HealthReport report)
    {
        var result = new
        {
            Status = report.Status.ToString(),
            Duration = report.TotalDuration,
            Info = report.Entries
                .Select(healthCheck =>
                    new
                    {
                        healthCheck.Key,
                        healthCheck.Value.Tags,
                        healthCheck.Value.Duration,
                        Status = Enum.GetName(typeof(HealthStatus), healthCheck.Value.Status),
                        Error = healthCheck.Value.Exception?.Message,
                    })
                .ToList(),
        }.ToJson();

        context.Response.ContentType = MediaTypeNames.Application.Json;

        return context.Response.WriteAsync(result);
    }
}
