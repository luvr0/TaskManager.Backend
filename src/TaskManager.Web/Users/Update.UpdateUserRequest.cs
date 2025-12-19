using System.ComponentModel.DataAnnotations;

namespace TaskManager.Web.Users;

public class UpdateUserRequest
{
  public const string Route = "/users/{UserId:int}";

  [Required]
  public int UserId { get; set; }

  public string? Name { get; set; }
  public string? Email { get; set; }
  public string? Password { get; set; }
}
