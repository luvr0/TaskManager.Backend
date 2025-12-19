using TaskManager.Core.UserAggregate;
using TaskManager.Core.UserAggregate.Specifications;

namespace TaskManager.UseCases.Authentication.Logout;

public sealed class LogoutHandler(IRepository<User> repository, ILogger<LogoutHandler> logger)
  : ICommandHandler<LogoutCommand, Result>
{
  public async ValueTask<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
  {
    if (string.IsNullOrWhiteSpace(request.RefreshToken))
    {
      return Result.Invalid(new[]
      {
        new ValidationError
        {
          Identifier = nameof(request.RefreshToken),
          ErrorMessage = "Refresh token is required."
        }
      });
    }

    var spec = new UserByRefreshTokenSpec(request.RefreshToken);
    var user = await repository.FirstOrDefaultAsync(spec, cancellationToken);

    if (user is null)
    {
      logger.LogInformation("Logout requested with an unknown refresh token");
      return Result.Success();
    }

    user.ClearRefreshToken();
    await repository.UpdateAsync(user, cancellationToken);

    logger.LogInformation("User {UserId} logged out successfully", user.Id.Value);

    return Result.Success();
  }
}
