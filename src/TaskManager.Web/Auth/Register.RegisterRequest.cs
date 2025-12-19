using System.ComponentModel.DataAnnotations;

namespace TaskManager.Web.Auth;

public sealed class RegisterRequest
{
  public const string Route = "/auth/register";

  [Required]
  public string Name { get; set; } = string.Empty;

  [Required]
  [EmailAddress]
  public string Email { get; set; } = string.Empty;

  [Required]
  public string Password { get; set; } = string.Empty;
}
