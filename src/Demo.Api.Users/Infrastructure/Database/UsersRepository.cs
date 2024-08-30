using Api.Users.Domain;
using Api.Users.Infrastructure.Cache;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Driver;

namespace Api.Users.Infrastructure.Database;

public sealed class UsersRepository(
    IMongoCollection<User> collection,
    IDistributedCache cache) : IUsersRepository
{
    private static readonly DistributedCacheEntryOptions _cacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30),
        SlidingExpiration = TimeSpan.FromSeconds(15)
    };

    private readonly IMongoCollection<User> _collection = collection;
    private readonly IDistributedCache _cache = cache;

    public async Task<IEnumerable<User>> ListAsync(CancellationToken cancellationToken = default)
    {
        var cursor = await _collection.FindAsync(
            f => true,
            cancellationToken: cancellationToken);

        return cursor.ToEnumerable(cancellationToken);
    }

    public async Task<User?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var key = $"{nameof(User)}:{id}";
        var snapshot = await _cache.GetAsync<User.Snapshot>(key, cancellationToken);

        if(snapshot is not null)
        {
            return User.RestoreSnapshot(snapshot);
        }

        var cursor = await _collection.FindAsync(
        f => f.Id == id,
        cancellationToken: cancellationToken);

        var user = await cursor.SingleOrDefaultAsync(cancellationToken);
        if(user is not null)
        {
            await _cache.SetAsync(
                key,
                user.ToSnapshot(),
                _cacheOptions,
                cancellationToken);
        }

        return user;
    }

    public Task AddAsync(User user, CancellationToken cancellationToken = default)
        => _collection.InsertOneAsync(
            document: user,
            options: null,
            cancellationToken: cancellationToken);

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        await _invalidateCache(user.Id, cancellationToken);

        await _collection.ReplaceOneAsync(
            filter: f => f.Id == user.Id,
            options: new ReplaceOptions { IsUpsert = false }, // Does not create user if does not exist
            replacement: user,
            cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _invalidateCache(id, cancellationToken);

        await _collection.DeleteOneAsync(
            filter: f => f.Id == id,
            options: null,
            cancellationToken: cancellationToken);
    }

    public async Task<bool> AnyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cursor = await _collection.FindAsync(
            f => f.Id == id,
            cancellationToken: cancellationToken);

        return await cursor.AnyAsync(cancellationToken);
    }

    private async Task _invalidateCache(Guid id, CancellationToken cancellationToken)
    {
        var key = $"{nameof(User)}:{id}";
        await _cache.RemoveAsync(key, cancellationToken);
    }
}
