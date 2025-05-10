namespace Coda.RoundRobin.Infrastructure.Cache;

using Coda.RoundRobin.Infrastructure.Cache.Interfaces;
using Coda.RoundRobin.Infrastructure.Cache.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

internal static class DependencyInjection
{
    public static IServiceCollection AddCache(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        var redisOptions = configuration.GetSection(nameof(RedisOptions)).Get<RedisOptions>();
        redisOptions ??= new RedisOptions();

        services.AddSingleton(redisOptions);

        if (environment.IsDevelopment())
        {
            services.AddMemoryCache();

            services.AddScoped<ICacheService, CacheServiceMock>();
        }
        else
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisOptions.ConnectionString;
                options.InstanceName = redisOptions.InstanceName;
            });

            services.AddScoped<ICacheService, CacheService>();
        }

        return services;
    }
}
