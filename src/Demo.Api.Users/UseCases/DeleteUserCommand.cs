using System;
using System.Threading;
using System.Threading.Tasks;
using Api.Users.Domain;
using BuildingBlocks.Contracts.Exceptions;

namespace Api.Users.UseCases;

public sealed class DeleteUserCommand(IUsersRepository repository)
{
    private readonly IUsersRepository _repository = repository;

    public async Task HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        ExceptionFactory.ProbablyThrow<DeleteUserCommand>();

        if(!await _repository.AnyAsync(id, cancellationToken))
        {
            throw new UserNotFoundException(id);
        }

        await _repository.DeleteAsync(id, cancellationToken);
    }
}
