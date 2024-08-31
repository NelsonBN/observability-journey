using Api.Notifications.Infrastructure.Database;
using Api.Notifications.Infrastructure.Grpc;
using Api.Notifications.Infrastructure.Http;
using Api.Notifications.Infrastructure.MessageBus;
using Api.Notifications.Infrastructure.Observability;
using Api.Notifications.Infrastructure.Schedules;
using Api.Notifications.Infrastructure.Storage;
using Api.Notifications.Infrastructure.UsersApi;
using BuildingBlocks.Observability;

ContinuousProfiling.Setup();

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<Program>());

builder.Services
    .AddDatabase()
    .AddGrpcServer()
    .AddHttp()
    .AddMessageBus()
    .AddUsersApi()
    .AddStorage()
    .AddSchedules()
    .AddObservability();


var app = builder.Build();

app.UseHttp();
app.AddObservability();
app.UseGrpcServer();

await app.RunAsync();
