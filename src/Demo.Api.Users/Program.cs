using Api.Users.Infrastructure.Cache;
using Api.Users.Infrastructure.Database;
using Api.Users.Infrastructure.Http;
using Api.Users.Infrastructure.NotificationsApi;
using Api.Users.Infrastructure.Observability;
using Api.Users.UseCases;
using BuildingBlocks.Observability;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

ContinuousProfiling.Setup();

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services
    .AddTransient<GetUsersQuery>()
    .AddTransient<GetUserQuery>()
    .AddTransient<GetUserNotificationsTotalsQuery>()
    .AddTransient<CreateUserCommand>()
    .AddTransient<UpdateUserCommand>()
    .AddTransient<DeleteUserCommand>();

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
