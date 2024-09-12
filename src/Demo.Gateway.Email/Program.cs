using BuildingBlocks.Observability;
using Gateway.Email.Infrastructure.MessageBus;
using Gateway.Email.Infrastructure.Observability;
using Gateway.Email.Infrastructure.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

ContinuousProfiling.Setup();

var builder = Host.CreateDefaultBuilder(args);

builder
    .ConfigureWebHostDefaults(builder =>
        builder.Configure(app => app.AddObservability()))
    .ConfigureServices((_, services)
        => services
            .AddMessageBus()
            .AddStorage()
            .AddObservability());



var app = builder.Build();

await app.RunAsync();
