using TaskManager.Core.UserAggregate;

namespace TaskManager.UseCases.Authentication.Login;

public sealed record AuthenticateUserCommand(UserEmail Email, string Password)
  : ICommand<Result<AuthTokensDto>>;
