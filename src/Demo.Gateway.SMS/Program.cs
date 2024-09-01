using BuildingBlocks.Observability;
using Gateway.SMS.Infrastructure.MessageBus;
using Gateway.SMS.Infrastructure.Observability;
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
            .AddObservability());



var app = builder.Build();

await app.RunAsync();
