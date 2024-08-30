using Api.Users.Infrastructure.Database;
using Api.Users.Infrastructure.Http;
using Api.Users.Infrastructure.NotificationsApi;
using Api.Users.Infrastructure.Observability;
using Common.Observability;

ContinuousProfiling.Setup();

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<Program>());

builder.Services
    .AddDatabase()
    .AddGrpcClient();


builder.Services.AddObservability();
builder.Services.AddHttp();



var app = builder.Build();

app.UseHttp();
app.AddObservability();

await app.RunAsync();
