using TaskManager.Core.UserAggregate;

namespace TaskManager.UnitTests.Core.UserAggregate;

public class UserTests
{
  [Fact]
  public void IsRefreshTokenValid_ReturnsFalse_WhenTokenMissingOrExpired()
  {
    var user = CreateUser();

    user.IsRefreshTokenValid().ShouldBeFalse();

    user.SetRefreshToken(RefreshToken.From("token"), DateTime.UtcNow.AddMinutes(-1));
    user.IsRefreshTokenValid().ShouldBeFalse();
  }

  [Fact]
  public void IsRefreshTokenValid_ReturnsTrue_WhenTokenIsActive()
  {
    var user = CreateUser();
    var refreshToken = RefreshToken.From("token-value");
    var expiresAt = DateTime.UtcNow.AddMinutes(5);

    user.SetRefreshToken(refreshToken, expiresAt);

    user.IsRefreshTokenValid().ShouldBeTrue();
    user.RefreshToken.ShouldBe(refreshToken);
    user.RefreshTokenExpiryTime.ShouldBe(expiresAt);
  }

  private static User CreateUser() => new(
    UserName.From("Test User"),
    UserEmail.From("tester@gmail.com"),
    UserPassword.From("hashed-password"));
}
