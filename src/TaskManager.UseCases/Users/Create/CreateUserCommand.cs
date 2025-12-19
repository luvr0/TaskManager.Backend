using TaskManager.Core.UserAggregate;

namespace TaskManager.UseCases.Users.Create;

public record CreateUserCommand(
  UserName Name,
  UserEmail Email,
  string Password
) : ICommand<Result<UserId>>;
