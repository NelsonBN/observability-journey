using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Api.Notifications.Domain;
using Api.Notifications.DTOs;
using BuildingBlocks.Contracts.Exceptions;

namespace Api.Notifications.Infrastructure.UsersApi;

public sealed class UsersService(HttpClient client) : IUsersService
{
    private readonly HttpClient _client = client;

    public async Task<UserResponse> GetUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var response = await _client.GetAsync($"users/{userId}", cancellationToken);
        if(response.IsSuccessStatusCode)
        {
            var user = await response.Content.ReadFromJsonAsync<UserResponse>(cancellationToken);
            if(user is not null)
            {
                return user;
            }
        }

        if(response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new UserNotFoundException(userId);
        }

        throw new InvalidOperationException(
            $"The file was not uploaded. Status: '{response.StatusCode}', Reason: '{response?.ReasonPhrase}'",
            new Exception(JsonSerializer.Serialize(response)));
    }
}
