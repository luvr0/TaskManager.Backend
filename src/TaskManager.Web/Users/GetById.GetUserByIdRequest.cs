using System.ComponentModel.DataAnnotations;

namespace TaskManager.Web.Users;

public class GetUserByIdRequest
{
  public const string Route = "/users/{UserId:int}";

  [Required]
  public int UserId { get; set; }
}
