using TaskManager.Core.UserAggregate;
using TaskManager.Core.UserAggregate.Specifications;
using TaskManager.Core.Interfaces;

namespace TaskManager.UseCases.Users.Update;

public class UpdateUserHandler(
  IRepository<User> repository,
  IPasswordHasher passwordHasher)
  : ICommandHandler<UpdateUserCommand, Result<UserId>>
{
  public async ValueTask<Result<UserId>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
  {
    var spec = new UserByIdSpec(UserId.From(request.UserId));
    var user = await repository.FirstOrDefaultAsync(spec, cancellationToken);

    if (user == null)
    {
      return Result<UserId>.NotFound();
    }

    if (request.Name.HasValue)
    {
      user.UpdateName(request.Name.Value);
    }

    if (request.Email.HasValue)
    {
      user.UpdateEmail(request.Email.Value);
    }

    if (!string.IsNullOrEmpty(request.Password))
    {
      var hashedPassword = passwordHasher.HashPassword(request.Password);
      user.UpdatePassword(UserPassword.From(hashedPassword));
    }


    await repository.UpdateAsync(user, cancellationToken);
    return Result<UserId>.Success(user.Id);
  }
}
