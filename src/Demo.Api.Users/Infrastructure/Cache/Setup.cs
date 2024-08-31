using BuildingBlocks;
using StackExchange.Redis;

namespace Api.Users.Infrastructure.Cache;

public static class Setup
{
    public static IServiceCollection AddCache(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionMultiplexer = ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!);
        services
            .AddSingleton(connectionMultiplexer)
            .AddStackExchangeRedisCache(options =>
            {
                options.InstanceName = AppDetails.Name;
                options.ConnectionMultiplexerFactory = () => Task.FromResult<IConnectionMultiplexer>(connectionMultiplexer);
            });

        return services;
    }
}
