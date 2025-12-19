namespace TaskManager.Web.Columns;

public class GetColumnByIdRequest
{
  public const string Route = "/Boards/{BoardId}/Columns/{ColumnId}";
  
  public int ColumnId { get; set; }
  public int BoardId { get; set; }
}
