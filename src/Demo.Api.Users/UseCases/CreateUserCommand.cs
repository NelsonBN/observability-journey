using Api.Users.Domain;
using Api.Users.DTOs;
using Common.Exceptions;
using MediatR;

namespace Api.Users.UseCases;

public sealed record CreateUserCommand(UserRequest Request) : IRequest<Guid>
{
    internal sealed class Handler(IUsersRepository repository) : IRequestHandler<CreateUserCommand, Guid>
    {
        private readonly IUsersRepository _repository = repository;

        public async Task<Guid> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            ExceptionFactory.ProbablyThrow<Handler>(35);

            var user = User.Create(
                command.Request.Name,
                command.Request.Email,
                command.Request.Phone);

            await _repository.AddAsync(user, cancellationToken);

            return user.Id;
        }
    }
}
