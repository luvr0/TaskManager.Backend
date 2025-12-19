using System.ComponentModel.DataAnnotations;

namespace TaskManager.Web.Columns;

public class CreateColumnRequest
{
  public const string Route = "/Boards/{BoardId}/Columns";

  [Required]
  public string Name { get; set; } = String.Empty;
  
  [Required]
  public int BoardId { get; set; }
}
