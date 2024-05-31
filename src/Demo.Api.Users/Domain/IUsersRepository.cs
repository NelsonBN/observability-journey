namespace Api.Users.Domain;

public interface IUsersRepository
{
    Task<IEnumerable<User>> ListAsync(CancellationToken cancellationToken = default);
    Task<User?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Guid id, CancellationToken cancellationToken = default);
}
