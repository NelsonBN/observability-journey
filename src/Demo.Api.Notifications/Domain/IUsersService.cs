using Api.Notifications.DTOs;

namespace Api.Notifications.Domain;

public interface IUsersService
{
    Task<UserResponse?> GetUserAsync(Guid userId, CancellationToken cancellationToken);
}
