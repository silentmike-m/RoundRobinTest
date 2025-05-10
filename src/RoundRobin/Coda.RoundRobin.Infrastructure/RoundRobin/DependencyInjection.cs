namespace Coda.RoundRobin.Infrastructure.RoundRobin;

using System.Diagnostics.CodeAnalysis;
using Coda.RoundRobin.Infrastructure.RoundRobin.Interfaces;
using Coda.RoundRobin.Infrastructure.RoundRobin.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[ExcludeFromCodeCoverage]
internal static class DependencyInjection
{
    public static IServiceCollection AddRoundRobin(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RoundRobinOptions>(configuration.GetSection(nameof(RoundRobinOptions)));

        services.AddScoped<IRoundRobinService, RoundRobinService>();

        services.AddScoped<IRetryPolicyFactory, RetryPolicyFactory>();

        services.AddSingleton<IEndpointResolver, EndpointResolver>();

        services.AddHttpClient();

        return services;
    }
}
