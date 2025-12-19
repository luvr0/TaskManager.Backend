using System.ComponentModel.DataAnnotations;

namespace TaskManager.Web.Auth;

public class LoginRequest
{
  public const string Route = "/auth/login";

  [Required]
  [EmailAddress]
  public string Email { get; set; } = string.Empty;

  [Required]
  public string Password { get; set; } = string.Empty;
}
