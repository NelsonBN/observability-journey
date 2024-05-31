using Api.Users.Domain;
using Api.Users.DTOs;
using Common.Exceptions;
using MediatR;

namespace Api.Users.UseCases;

public sealed record UpdateUserCommand(Guid Id, UserRequest Request) : IRequest
{
    internal sealed class Handler(IUsersRepository repository) : IRequestHandler<UpdateUserCommand>
    {
        private readonly IUsersRepository _repository = repository;

        public async Task Handle(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            var user = await _repository.GetAsync(command.Id, cancellationToken);
            if(user is null)
            {
                throw new UserNotFoundException(command.Id);
            }

            user.Update(
                command.Request.Name,
                command.Request.Email,
                command.Request.Phone);

            await _repository.UpdateAsync(user, cancellationToken);
        }
    }
}
