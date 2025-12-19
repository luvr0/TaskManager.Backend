using System.ComponentModel.DataAnnotations;

namespace TaskManager.Web.Users;

public class CreateUserRequest
{
  public const string Route = "/users";

  [Required]
  public string Name { get; set; } = string.Empty;

  [Required]
  public string Email { get; set; } = string.Empty;

  [Required]
  public string Password { get; set; } = string.Empty;
}
