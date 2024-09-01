using System;

namespace Api.Notifications.DTOs;

public sealed record UserResponse(
    Guid Id,
    string Name,
    string? Email,
    string? Phone);
