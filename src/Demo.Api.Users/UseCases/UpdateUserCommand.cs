using Api.Users.Domain;
using Api.Users.DTOs;
using BuildingBlocks.Exceptions;
using MediatR;

namespace Api.Users.UseCases;

public sealed record UpdateUserCommand(IUsersRepository Repository) : IRequest
{
    private readonly IUsersRepository _repository = Repository;


    public async Task Handle(Guid id, UserRequest request, CancellationToken cancellationToken)
    {
        ExceptionFactory.ProbablyThrow<UpdateUserCommand>(35);

        var user = await _repository.GetAsync(id, cancellationToken);
        if(user is null)
        {
            throw new UserNotFoundException(id);
        }

        user.Update(
            request.Name,
            request.Email,
            request.Phone);

        await _repository.UpdateAsync(user, cancellationToken);
    }
}
