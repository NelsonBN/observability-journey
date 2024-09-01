using System;

namespace BuildingBlocks.Contracts.Exceptions;

public sealed class UserNotFoundException(Guid Id)
    : Exception($"User with id '{Id}' was not found")
{ }
