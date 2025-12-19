namespace TaskManager.Web.Columns;

public class DeleteColumnRequest
{
  public const string Route = "/Boards/{BoardId}/Columns/{ColumnId}";
  
  public int ColumnId { get; set; }
  public int BoardId { get; set; }
}
