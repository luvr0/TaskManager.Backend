using TaskManager.Core.Interfaces;
using TaskManager.Core.UserAggregate;
using TaskManager.Core.UserAggregate.Specifications;

namespace TaskManager.UseCases.Authentication.Refresh;

public sealed class RefreshTokenHandler(
  IRepository<User> repository,
  ITokenService tokenService,
  ILogger<RefreshTokenHandler> logger)
  : ICommandHandler<RefreshTokenCommand, Result<AuthTokensDto>>
{
  public async ValueTask<Result<AuthTokensDto>> Handle(
    RefreshTokenCommand request,
    CancellationToken cancellationToken)
  {
    var userSpec = new UserByRefreshTokenSpec(request.RefreshToken);
    var user = await repository.FirstOrDefaultAsync(userSpec, cancellationToken);

    if (user is null)
    {
      logger.LogWarning("Refresh token rejected because it does not match any user");
      return Result<AuthTokensDto>.Unauthorized();
    }

    if (!user.IsRefreshTokenValid())
    {
      user.ClearRefreshToken();
      await repository.UpdateAsync(user, cancellationToken);

      return Result<AuthTokensDto>.Invalid(new[]
      {
        new ValidationError
        {
          Identifier = nameof(User.RefreshToken),
          ErrorMessage = "Refresh token has expired."
        }
      });
    }

    var accessToken = tokenService.GenerateAccessToken(user);
    var newRefreshToken = tokenService.GenerateRefreshToken();
    var refreshExpiry = DateTime.UtcNow.AddDays(Constants.REFRESH_TOKEN_LIFETIME_DAYS);

    user.SetRefreshToken(newRefreshToken, refreshExpiry);
    await repository.UpdateAsync(user, cancellationToken);

    return Result<AuthTokensDto>.Success(
      AuthTokensDto.From(user, accessToken, newRefreshToken, refreshExpiry));
  }
}
