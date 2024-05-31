using Api.Users.Domain;
using MongoDB.Driver;

namespace Api.Users.Infrastructure.Database;

public sealed class UsersRepository(IMongoCollection<User> collection) : IUsersRepository
{
    private readonly IMongoCollection<User> _collection = collection;

    public async Task<IEnumerable<User>> ListAsync(CancellationToken cancellationToken = default)
    {
        var cursor = await _collection.FindAsync(
            f => true,
            cancellationToken: cancellationToken);

        return cursor.ToEnumerable(cancellationToken);
    }

    public async Task<User?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cursor = await _collection.FindAsync(
            f => f.Id == id,
            cancellationToken: cancellationToken);

        return await cursor.SingleOrDefaultAsync(cancellationToken);
    }

    public Task AddAsync(User user, CancellationToken cancellationToken = default)
        => _collection.InsertOneAsync(
            document: user,
            options: null,
            cancellationToken: cancellationToken);

    public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
        => _collection.ReplaceOneAsync(
            filter: f => f.Id == user.Id,
            options: new ReplaceOptions { IsUpsert = false }, // Does not create user if does not exist
            replacement: user,
            cancellationToken: cancellationToken);

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        => _collection.DeleteOneAsync(
            filter: f => f.Id == id,
            options: null,
            cancellationToken: cancellationToken);

    public async Task<bool> AnyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cursor = await _collection.FindAsync(
            f => f.Id == id,
            cancellationToken: cancellationToken);

        return await cursor.AnyAsync(cancellationToken);
    }
}
