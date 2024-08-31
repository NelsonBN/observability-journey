using Api.Users.Domain;
using Api.Users.DTOs;
using BuildingBlocks.Exceptions;

namespace Api.Users.UseCases;

public sealed record CreateUserCommand(IUsersRepository Repository)
{
    private readonly IUsersRepository _repository = Repository;

    public async Task<Guid> Handle(UserRequest request, CancellationToken cancellationToken)
    {
        ExceptionFactory.ProbablyThrow<CreateUserCommand>(35);

        var user = User.Create(
            request.Name,
            request.Email,
            request.Phone);

        await _repository.AddAsync(user, cancellationToken);

        return user.Id;
    }

}
