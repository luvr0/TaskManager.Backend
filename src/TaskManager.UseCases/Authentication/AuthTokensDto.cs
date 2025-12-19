using TaskManager.Core.UserAggregate;

namespace TaskManager.UseCases.Authentication;

public sealed record AuthTokensDto(
  int UserId,
  string Email,
  string AccessToken,
  string RefreshToken,
  DateTime RefreshTokenExpiresAt)
{
  public static AuthTokensDto From(
    User user,
    string accessToken,
    RefreshToken refreshToken,
    DateTime refreshTokenExpiresAt)
  {
    return new AuthTokensDto(
      user.Id.Value,
      user.Email.Value,
      accessToken,
      refreshToken.Value,
      refreshTokenExpiresAt);
  }
}
