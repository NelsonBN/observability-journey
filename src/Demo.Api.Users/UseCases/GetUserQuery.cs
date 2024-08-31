using Api.Users.Domain;
using Api.Users.DTOs;
using BuildingBlocks.Exceptions;

namespace Api.Users.UseCases;

public sealed record GetUserQuery(IUsersRepository Repository)
{
    private readonly IUsersRepository _repository = Repository;

    public async Task<UserResponse> Handle(Guid id, CancellationToken cancellationToken)
    {
        ExceptionFactory.ProbablyThrow<GetUserQuery>(35);

        var user = await _repository.GetAsync(id, cancellationToken);
        if(user is null)
        {
            throw new UserNotFoundException(id);
        }

        return user;
    }
}
