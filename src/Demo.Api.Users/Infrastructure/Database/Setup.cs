using Api.Users.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;

namespace Api.Users.Infrastructure.Database;

public static class Setup
{
    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        // Changes Id format from "ObjectID with 96 bits" to "GUID with 128 bits"
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));

        services.AddSingleton(sp =>
            new MongoUrl(sp.GetRequiredService<IConfiguration>()
                .GetConnectionString("MongoDB")!));

        services.AddSingleton<IMongoClient>(sp =>
        {
            var mongoUrl = sp.GetRequiredService<MongoUrl>();
            var mongoClientSettings = MongoClientSettings.FromUrl(mongoUrl);
            mongoClientSettings.ClusterConfigurator = cb => cb
                .Subscribe(new DiagnosticsActivityEventSubscriber(new() { CaptureCommandText = true }));

            return new MongoClient(mongoClientSettings);
        });

        services.AddSingleton(sp =>
        {
            var mongoUrl = sp.GetRequiredService<MongoUrl>();
            var mongoClient = sp.GetRequiredService<IMongoClient>();

            var database = mongoClient.GetDatabase(mongoUrl.DatabaseName);

            return database.GetCollection<User>(nameof(User));
        });

        services.AddScoped<IUsersRepository, UsersRepository>();

        return services;
    }

    public static IHealthChecksBuilder AddDatabase(this IHealthChecksBuilder builder)
        => builder
            // TODO: temporary disabled. HealthChecks isn't compatible with Mongo.Driver to 3.0.0 -> https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/pull/2312
            //.AddMongoDb(
            //    mongodbConnectionStringFactory: sp => sp.GetRequiredService<IConfiguration>().GetConnectionString("MongoDB")!,
            //    name: "MongoDB",
            //    failureStatus: HealthStatus.Unhealthy)
            .AddRedis(
                name: "Redis",
                connectionStringFactory: (sp) => sp.GetRequiredService<IConfiguration>().GetConnectionString("Redis")!,
                failureStatus: HealthStatus.Unhealthy);
}
