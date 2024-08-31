using BuildingBlocks.Observability;
using Gateway.Email.Infrastructure.MessageBus;
using Gateway.Email.Infrastructure.Observability;
using Microsoft.AspNetCore.Hosting;

ContinuousProfiling.Setup();

var builder = Host.CreateDefaultBuilder(args);

builder
    .ConfigureWebHostDefaults(builder =>
        builder.Configure(app => app.AddObservability()))
    .ConfigureServices((_, services) =>
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<Program>());

        services.AddMessageBus();

        services.AddObservability();
    });



var app = builder.Build();

await app.RunAsync();
