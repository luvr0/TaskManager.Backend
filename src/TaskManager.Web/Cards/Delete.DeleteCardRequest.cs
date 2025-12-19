namespace TaskManager.Web.Cards;

public class DeleteCardRequest
{
  public const string Route = "/Boards/{BoardId}/Cards/{CardId}";
  
  public int CardId { get; set; }
  public int BoardId { get; set; }
}
