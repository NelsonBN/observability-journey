using Api.Users.Domain;
using BuildingBlocks.Exceptions;

namespace Api.Users.UseCases;

public sealed record DeleteUserCommand(IUsersRepository Repository)
{
    private readonly IUsersRepository _repository = Repository;

    public async Task Handle(Guid id, CancellationToken cancellationToken)
    {
        ExceptionFactory.ProbablyThrow<DeleteUserCommand>(35);

        if(!await _repository.AnyAsync(id, cancellationToken))
        {
            throw new UserNotFoundException(id);
        }

        await _repository.DeleteAsync(id, cancellationToken);
    }
}
