using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Api.Users.Domain;
using Api.Users.DTOs;
using BuildingBlocks.Contracts.Exceptions;

namespace Api.Users.UseCases;

public sealed record GetUsersQuery(IUsersRepository Repository)
{
    private readonly IUsersRepository _repository = Repository;

    public async Task<IEnumerable<UserResponse>> HandleAsync(CancellationToken cancellationToken)
    {
        ExceptionFactory.ProbablyThrow<GetUsersQuery>();

        var users = await _repository.ListAsync(cancellationToken);

        var result = users.Select(n => (UserResponse)n);

        return result;
    }
}
