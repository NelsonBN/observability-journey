using System;
using System.Collections.Generic;
using BuildingBlocks.Contracts;

namespace Api.Notifications.Domain;

public sealed class Notification
{
    private readonly List<Message> _domainEvents = [];

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Body { get; private set; } = default!;

    public string? Email { get; private set; }
    public NotificationStatus EmailNotificationStatus { get; private set; }

    public string? Phone { get; private set; }
    public NotificationStatus PhoneNotificationStatus { get; private set; }

    public int Version { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private Notification() { }


    public void SMSSent()
    {
        PhoneNotificationStatus = NotificationStatus.Sent;
        Version++;
    }

    public void SMSFailed()
    {
        PhoneNotificationStatus = NotificationStatus.Failed;
        Version++;
    }

    public void EmailSent()
    {
        EmailNotificationStatus = NotificationStatus.Sent;
        Version++;
    }

    public void EmailFailed()
    {
        EmailNotificationStatus = NotificationStatus.Failed;
        Version++;
    }


    public IReadOnlyCollection<Message> GetDomainEvents()
    {
        var events = _domainEvents
            .ToArray()
            .AsReadOnly();

        _domainEvents.Clear();

        return events;
    }

    public static Notification Create(Guid userId, string body, string? email, string? phone)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(body, nameof(body));


        if(string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(phone))
        {
            throw new ArgumentException("Email or phone must be provided");
        }

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Version = 1,
            Body = body,
            CreatedAt = DateTime.UtcNow
        };


        if(string.IsNullOrWhiteSpace(email))
        {
            notification.EmailNotificationStatus = NotificationStatus.NoSent;
        }
        else
        {
            notification.Email = email;
            notification.EmailNotificationStatus = NotificationStatus.Pending;

            notification._domainEvents.Add(new()
            {
                Context = "notifications",
                Type = "email.requested",
                AggregateId = notification.Id,
                Data = new Dictionary<string, object>
                {
                    [nameof(notification.Email)] = notification.Email,
                    [nameof(notification.Body)] = notification.Body
                }
            });
        }


        if(string.IsNullOrWhiteSpace(phone))
        {
            notification.PhoneNotificationStatus = NotificationStatus.NoSent;
        }
        else
        {
            notification.Phone = phone;
            notification.PhoneNotificationStatus = NotificationStatus.Pending;

            notification._domainEvents.Add(new()
            {
                Context = "notifications",
                Type = "sms.requested",
                AggregateId = notification.Id,
                Data = new Dictionary<string, object>
                {
                    [nameof(notification.Phone)] = notification.Phone,
                    [nameof(notification.Body)] = notification.Body
                }
            });
        }

        return notification;
    }
}
