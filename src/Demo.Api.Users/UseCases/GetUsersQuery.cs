using Api.Users.Domain;
using Api.Users.DTOs;
using BuildingBlocks.Exceptions;
using MediatR;

namespace Api.Users.UseCases;

public sealed record GetUsersQuery(IUsersRepository Repository) : IRequest<IEnumerable<UserResponse>>
{
    private readonly IUsersRepository _repository = Repository;

    public async Task<IEnumerable<UserResponse>> Handle(CancellationToken cancellationToken)
    {
        ExceptionFactory.ProbablyThrow<GetUsersQuery>(35);

        var users = await _repository.ListAsync(cancellationToken);

        var result = users.Select(n => (UserResponse)n);

        return result;
    }
}
