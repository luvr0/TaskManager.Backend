namespace TaskManager.Web.Columns;

public class UpdateColumnRequest
{
  public const string Route = "/Boards/{BoardId}/Columns/{ColumnId}";
  
  public int ColumnId { get; set; }
  public int BoardId { get; set; }
  public string? Name { get; set; }
}
