using System;
using Api.Notifications.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Api.Notifications.Infrastructure.Database;

public sealed class DataContext(ILoggerFactory LoggerFactory, IConfiguration Configuration) : DbContext
{
    public DbSet<Notification> Notifications { get; set; } = default!;
    public DbSet<ReportState> ReportState { get; set; } = default!;

    private readonly ILoggerFactory _loggerFactory = LoggerFactory;
    private readonly IConfiguration _configuration = Configuration;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .UseNpgsql(_configuration.GetConnectionString("Postgres")!)
            .UseLoggerFactory(_loggerFactory)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder
            .Entity<Notification>(notification =>
            {
                notification
                    .ToTable("notification")
                    .HasKey(product => product.Id);

                notification.Property(p => p.Id)
                            .HasColumnName("id");

                notification.Property(p => p.UserId)
                            .HasColumnName("user_id");

                notification.Property(p => p.Version)
                            .HasColumnName("version");

                notification.Property(p => p.Message)
                            .HasColumnName("message");

                notification.Property(p => p.Email)
                            .HasColumnName("email");

                notification.Property(p => p.EmailNotificationStatus)
                            .HasColumnName("email_notification_status")
                            .HasConversion(
                                v => v.ToString(),
                                v => (NotificationStatus)Enum.Parse(typeof(NotificationStatus), v, true));

                notification.Property(p => p.Phone)
                            .HasColumnName("phone");

                notification.Property(p => p.PhoneNotificationStatus)
                            .HasColumnName("phone_notification_status")
                            .HasConversion(
                                v => v.ToString(),
                                v => (NotificationStatus)Enum.Parse(typeof(NotificationStatus), v, true));

                notification.Property(p => p.CreatedAt)
                            .HasColumnName("created_at")
                            .HasColumnType("TIMESTAMPTZ")
                            .HasConversion(
                                v => v, // to the database (stored as UTC)
                                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
            })
            .Entity<ReportState>(reportState =>
            {
                reportState
                    .ToTable("report_state")
                    .HasKey(product => product.Id);

                reportState.Property(p => p.Id)
                           .HasColumnName("id");

                reportState
                    .Property(p => p.LastGeneratedAt)
                    .HasColumnName("last_generated_at")
                    .HasColumnType("TIMESTAMPTZ")
                    .HasConversion(
                        v => v, // to the database (stored as UTC)
                        v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
            });
}
