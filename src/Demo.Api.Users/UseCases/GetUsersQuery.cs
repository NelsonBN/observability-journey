using Api.Users.Domain;
using Api.Users.DTOs;
using BuildingBlocks.Exceptions;
using MediatR;

namespace Api.Users.UseCases;

public sealed record GetUsersQuery : IRequest<IEnumerable<UserResponse>>
{
    public static GetUsersQuery Instance => new();

    internal sealed class Handler(IUsersRepository repository) : IRequestHandler<GetUsersQuery, IEnumerable<UserResponse>>
    {
        private readonly IUsersRepository _repository = repository;

        public async Task<IEnumerable<UserResponse>> Handle(GetUsersQuery query, CancellationToken cancellationToken)
        {
            ExceptionFactory.ProbablyThrow<Handler>(35);

            var users = await _repository.ListAsync(cancellationToken);

            var result = users.Select(n => (UserResponse)n);

            return result;
        }
    }
}
