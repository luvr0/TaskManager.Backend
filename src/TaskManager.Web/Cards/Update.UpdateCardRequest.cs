namespace TaskManager.Web.Cards;

public class UpdateCardRequest
{
  public const string Route = "/Boards/{BoardId}/Cards/{CardId}";
  
  public int CardId { get; set; }
  public int BoardId { get; set; }
  public string? Title { get; set; }
  public string? Description { get; set; }
  public string? Status { get; set; }
}
