using TaskManager.Core.UserAggregate;
using TaskManager.Core.UserAggregate.Specifications;

namespace TaskManager.UseCases.Users.Delete;

public class DeleteUserHandler(IRepository<User> repository)
  : ICommandHandler<DeleteUserCommand, Result>
{
  public async ValueTask<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
  {
    var spec = new UserByIdSpec(UserId.From(request.UserId));
    var user = await repository.FirstOrDefaultAsync(spec, cancellationToken);

    if (user == null)
    {
      return Result.NotFound();
    }

    await repository.DeleteAsync(user, cancellationToken);
    return Result.Success();
  }
}
