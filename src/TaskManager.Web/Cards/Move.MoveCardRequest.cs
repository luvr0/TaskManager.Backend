namespace TaskManager.Web.Cards;

public class MoveCardRequest
{
  public const string Route = "/Boards/{BoardId}/Cards/{CardId}/Move";
  
  public int CardId { get; set; }
  public int BoardId { get; set; }
  public int TargetColumnId { get; set; }
}
