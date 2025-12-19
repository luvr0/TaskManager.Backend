using TaskManager.Core.UserAggregate;

namespace TaskManager.UseCases.Users.Update;

public record UpdateUserCommand(
  int UserId,
  UserName? Name = null,
  UserEmail? Email = null,
  string? Password = null
) : ICommand<Result<UserId>>;
