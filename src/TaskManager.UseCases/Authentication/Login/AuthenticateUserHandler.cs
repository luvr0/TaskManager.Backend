using TaskManager.Core.Interfaces;
using TaskManager.Core.UserAggregate;
using TaskManager.Core.UserAggregate.Specifications;

namespace TaskManager.UseCases.Authentication.Login;

public sealed class AuthenticateUserHandler(
  IRepository<User> repository,
  IPasswordHasher passwordHasher,
  ITokenService tokenService,
  ILogger<AuthenticateUserHandler> logger)
  : ICommandHandler<AuthenticateUserCommand, Result<AuthTokensDto>>
{
  public async ValueTask<Result<AuthTokensDto>> Handle(
    AuthenticateUserCommand request,
    CancellationToken cancellationToken)
  {
    var userSpec = new UserByEmailSpec(request.Email);
    var user = await repository.FirstOrDefaultAsync(userSpec, cancellationToken);

    if (user is null)
    {
      logger.LogWarning("Authentication failed for {Email}", request.Email.Value);
      return Result<AuthTokensDto>.Unauthorized();
    }

    if (!passwordHasher.VerifyPassword(request.Password, user.PasswordHash.Value))
    {
      logger.LogWarning("Authentication failed for {Email} due to invalid credentials", request.Email.Value);
      return Result<AuthTokensDto>.Unauthorized();
    }

    var accessToken = tokenService.GenerateAccessToken(user);
    var refreshToken = tokenService.GenerateRefreshToken();
    var refreshExpiry = DateTime.UtcNow.AddDays(Constants.REFRESH_TOKEN_LIFETIME_DAYS);

    user.SetRefreshToken(refreshToken, refreshExpiry);
    await repository.UpdateAsync(user, cancellationToken);

    return Result<AuthTokensDto>.Success(
      AuthTokensDto.From(user, accessToken, refreshToken, refreshExpiry));
  }
}
