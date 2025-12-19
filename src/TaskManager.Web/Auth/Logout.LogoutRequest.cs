namespace TaskManager.Web.Auth;

public sealed class LogoutRequest
{
  public const string Route = "/auth/logout";

  public string? RefreshToken { get; set; }
}
