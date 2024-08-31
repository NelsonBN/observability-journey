using Api.Notifications.Domain;

namespace Api.Notifications.Infrastructure.Database;

public static class Setup
{
    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddDbContext<DataContext>()
                .AddScoped<INotificationsRepository, NotificationsRepository>()
                .AddScoped<IReportsRepository, ReportsRepository>();

        return services;
    }
}
