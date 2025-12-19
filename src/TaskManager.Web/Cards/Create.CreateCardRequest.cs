using System.ComponentModel.DataAnnotations;

namespace TaskManager.Web.Cards;

public class CreateCardRequest
{
  public const string Route = "/Boards/{BoardId}/Columns/{ColumnId}/Cards";

  [Required]
  public string Title { get; set; } = String.Empty;
  
  public string? Description { get; set; }
  
  [Required]
  public int ColumnId { get; set; }
  
  [Required]
  public int BoardId { get; set; }
}
