namespace TaskManager.Web.Auth;

public class RefreshTokenRequest
{
  public const string Route = "/auth/refresh";

  public string? RefreshToken { get; set; }
}
