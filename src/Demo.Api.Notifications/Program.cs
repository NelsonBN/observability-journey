using Api.Notifications.Infrastructure.Database;
using Api.Notifications.Infrastructure.Grpc;
using Api.Notifications.Infrastructure.Http;
using Api.Notifications.Infrastructure.MessageBus;
using Api.Notifications.Infrastructure.Observability;
using Api.Notifications.Infrastructure.UsersApi;
using BuildingBlocks.Observability;

ContinuousProfiling.Setup();

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<Program>());

builder.Services
    .AddDatabase()
    .AddGrpcServer()
    .AddMessageBus()
    .AddUsersApi();

builder.Services.AddObservability();

builder.Services.AddHttp();



var app = builder.Build();

app.UseHttp();
app.AddObservability();
app.UseGrpcServer();

await app.RunAsync();
