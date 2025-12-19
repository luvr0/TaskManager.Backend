using TaskManager.Core.UserAggregate;
using TaskManager.Core.UserAggregate.Specifications;
using TaskManager.Core.Interfaces;

namespace TaskManager.UseCases.Users.Create;

public class CreateUserHandler(
  IRepository<User> repository,
  IPasswordHasher passwordHasher)
  : ICommandHandler<CreateUserCommand, Result<UserId>>
{
  public async ValueTask<Result<UserId>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
  {
    var existingUserSpec = new UserByEmailSpec(request.Email);
    var existingUser = await repository.FirstOrDefaultAsync(existingUserSpec, cancellationToken);

    if (existingUser != null)
    {
      return Result<UserId>.Error("User with this email already exists");
    }

    if (!UserPassword.IsStrongPassword(request.Password))
    {
      return Result<UserId>.Error("Password does not meet complexity requirements. It must be at least 8 characters and include upper and lower case letters, a digit and a special character.");
    }

    var hashedPassword = passwordHasher.HashPassword(request.Password);

    var newUser = new User(
      request.Name,
      request.Email,
      UserPassword.From(hashedPassword)
    );

    var createdUser = await repository.AddAsync(newUser, cancellationToken);
    return Result<UserId>.Success(createdUser.Id);
  }
}
