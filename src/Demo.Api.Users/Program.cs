﻿using Api.Users.Infrastructure.Cache;
using Api.Users.Infrastructure.Database;
using Api.Users.Infrastructure.Http;
using Api.Users.Infrastructure.NotificationsApi;
using Api.Users.Infrastructure.Observability;
using BuildingBlocks.Observability;

ContinuousProfiling.Setup();

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<Program>());

builder.Services
    .AddDatabase()
    .AddCache(builder.Configuration)
    .AddGrpcClient();


builder.Services.AddObservability();
builder.Services.AddHttp();



var app = builder.Build();

app.UseHttp();
app.AddObservability();

await app.RunAsync();
