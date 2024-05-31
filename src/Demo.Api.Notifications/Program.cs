using Api.Notifications.Infrastructure.Cache;
using Api.Notifications.Infrastructure.Database;
using Api.Notifications.Infrastructure.Grpc;
using Api.Notifications.Infrastructure.Http;
using Api.Notifications.Infrastructure.MessageBus;
using Api.Notifications.Infrastructure.Observability;
using Api.Notifications.Infrastructure.UsersApi;
using Common.Observability;

ContinuousProfiling.Setup();

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<Program>());

builder.Services
    .AddDatabase()
    .AddCache(builder.Configuration)
    .AddGrpcServer()
    .AddMessageBus()
    .AddUsersApi();

builder.Logging.AddObservability();
builder.Services.AddObservability();

builder.Services.AddHttp();

var app = builder.Build();



app.AddObservability();
app.UseHttp();
app.UseGrpcServer();

await app.RunAsync();
