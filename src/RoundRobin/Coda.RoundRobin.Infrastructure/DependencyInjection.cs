﻿namespace Coda.RoundRobin.Infrastructure;

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Coda.RoundRobin.Infrastructure.Cache;
using Coda.RoundRobin.Infrastructure.HealthChecks;
using Coda.RoundRobin.Infrastructure.RoundRobin;
using Coda.RoundRobin.Infrastructure.RoundRobin.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        var assembly = Assembly.GetExecutingAssembly();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        services
            .AddHealthChecks(configuration)
            .AddCache(configuration, environment)
            .AddRoundRobin(configuration);

        return services;
    }

    public static void UseInfrastructure(this IServiceProvider serviceProvider)
    {
        var endpointResolver = serviceProvider.GetRequiredService<IEndpointResolver>();

        endpointResolver.InitializeEndpointsAsync(CancellationToken.None)
            .GetAwaiter()
            .GetResult();
    }
}
