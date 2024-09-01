using System;

namespace Api.Users.Domain;

public sealed class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string? Email { get; private set; }
    public string? Phone { get; private set; }

    private User() { }

    public void Update(string name, string? email, string? phone)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

        if(string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(phone))
        {
            throw new ArgumentException("At least one of email or phone must be provided");
        }

        Name = name;
        Email = email;
        Phone = phone;
    }

    public static User Create(string name, string? email, string? phone)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

        if(string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(phone))
        {
            throw new ArgumentException("At least one of email or phone must be provided");
        }

        return new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            Phone = phone
        };
    }

    public static User RestoreSnapshot(Snapshot snapshot)
        => new()
        {
            Id = snapshot.Id,
            Name = snapshot.Name,
            Email = snapshot.Email,
            Phone = snapshot.Phone
        };

    public Snapshot ToSnapshot()
        => new()
        {
            Id = Id,
            Name = Name,
            Email = Email,
            Phone = Phone
        };

    public sealed class Snapshot
    {
        public Guid Id { get; init; }
        public required string Name { get; init; }
        public string? Email { get; init; }
        public string? Phone { get; init; }
    }
}
