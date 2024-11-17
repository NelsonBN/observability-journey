using System;
using System.Threading;
using System.Threading.Tasks;
using Api.Users.Domain;
using Api.Users.DTOs;
using BuildingBlocks.Contracts.Exceptions;

namespace Api.Users.UseCases;

public sealed class UpdateUserCommand(IUsersRepository repository)
{
    private readonly IUsersRepository _repository = repository;


    public async Task HandleAsync(Guid id, UserRequest request, CancellationToken cancellationToken)
    {
        ExceptionFactory.ProbablyThrow<UpdateUserCommand>();

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
