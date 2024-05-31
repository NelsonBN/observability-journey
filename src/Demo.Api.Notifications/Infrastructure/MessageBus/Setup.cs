using Common.MessageBus;
using RabbitMQ.Client;

namespace Api.Notifications.Infrastructure.MessageBus;

public static class Setup
{
    public static IServiceCollection AddMessageBus(this IServiceCollection services)
    {
        services
            .ConfigureOptions<MessageBusOptions.Setup>()
            .AddSingleton<IConnectionFactory>(sp =>
                sp.GetRequiredService<IConfiguration>().GetSection(MessageBusOptions.Setup.SECTION_NAME).Get<ConnectionFactory>()!)
            .AddTransient(sp =>
                sp.GetRequiredService<IConnectionFactory>().CreateConnection())
            .AddTransient(sp =>
                sp.GetRequiredService<IConnection>().CreateModel())
            .AddTransient<IMessageBus, MessageBusServer>()
            .AddHostedService<ConsumerWorker>();

        return services;
    }
}
