namespace TaskManager.Web.Boards;

public class GetBoardByIdRequest
{
  public const string Route = "/Boards/{BoardId:int}";
  public static string BuildRoute(int boardId) => Route.Replace("{BoardId:int}", boardId.ToString());

  public int BoardId { get; set; }
}
