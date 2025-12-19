namespace TaskManager.Core.UserAggregate.Specifications;

public sealed class UserByRefreshTokenSpec : Specification<User>
{
  public UserByRefreshTokenSpec(string refreshTokenValue)
  {
    var refreshToken = RefreshToken.From(refreshTokenValue);

    Query.Where(user =>
      user.RefreshToken.HasValue &&
      user.RefreshToken.Value == refreshToken);
  }
}
