using TaskManager.Core.UserAggregate;

namespace TaskManager.Core.Interfaces;

public interface ITokenService
{
  string GenerateAccessToken(User user);
  RefreshToken GenerateRefreshToken();
  UserId? ValidateAccessToken(string token);
}
