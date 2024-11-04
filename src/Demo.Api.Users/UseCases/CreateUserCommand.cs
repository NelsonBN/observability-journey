using System;
using System.Threading;
using System.Threading.Tasks;
using Api.Users.Domain;
using Api.Users.DTOs;
using BuildingBlocks.Contracts.Exceptions;

namespace Api.Users.UseCases;

public sealed record CreateUserCommand(IUsersRepository Repository)
{
    private readonly IUsersRepository _repository = Repository;

    public async Task<Guid> HandleAsync(UserRequest request, CancellationToken cancellationToken)
    {
        ExceptionFactory.ProbablyThrow<CreateUserCommand>();

        var user = User.Create(
            request.Name,
            request.Email,
            request.Phone);

        await _repository.AddAsync(user, cancellationToken);

        return user.Id;
    }

}
