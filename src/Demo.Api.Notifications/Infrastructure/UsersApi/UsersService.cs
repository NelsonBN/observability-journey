using Api.Notifications.Domain;
using Api.Notifications.DTOs;

namespace Api.Notifications.Infrastructure.UsersApi;

public sealed class UsersService(HttpClient client) : IUsersService
{
    private readonly HttpClient _client = client;

    public async Task<UserResponse?> GetUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var response = await _client.GetAsync($"users/{userId}", cancellationToken);

        return response.IsSuccessStatusCode ?
            await response.Content.ReadFromJsonAsync<UserResponse>(cancellationToken) :
            null;
    }
}
