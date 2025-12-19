using System.ComponentModel.DataAnnotations;

namespace TaskManager.Web.Boards;

public class CreateBoardRequest
{
  public const string Route = "/Boards";

  [Required]
  public string Name { get; set; } = string.Empty;
}
