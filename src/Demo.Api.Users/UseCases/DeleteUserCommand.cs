using Api.Users.Domain;
using Common.Exceptions;
using MediatR;

namespace Api.Users.UseCases;

public sealed record DeleteUserCommand(Guid Id) : IRequest
{
    internal sealed class Handler(IUsersRepository repository) : IRequestHandler<DeleteUserCommand>
    {
        private readonly IUsersRepository _repository = repository;

        public async Task Handle(DeleteUserCommand command, CancellationToken cancellationToken)
        {
            if(!await _repository.AnyAsync(command.Id, cancellationToken))
            {
                throw new UserNotFoundException(command.Id);
            }

            await _repository.DeleteAsync(command.Id, cancellationToken);
        }
    }
}
