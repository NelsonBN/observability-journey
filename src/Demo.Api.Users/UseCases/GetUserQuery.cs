using Api.Users.Domain;
using Api.Users.DTOs;
using BuildingBlocks.Exceptions;
using MediatR;

namespace Api.Users.UseCases;

public sealed record GetUserQuery(Guid Id) : IRequest<UserResponse>
{
    internal sealed class Handler(IUsersRepository repository) : IRequestHandler<GetUserQuery, UserResponse>
    {
        private readonly IUsersRepository _repository = repository;

        public async Task<UserResponse> Handle(GetUserQuery query, CancellationToken cancellationToken)
        {
            ExceptionFactory.ProbablyThrow<Handler>(35);

            var user = await _repository.GetAsync(query.Id, cancellationToken);
            if(user is null)
            {
                throw new UserNotFoundException(query.Id);
            }

            return user;
        }
    }
}
