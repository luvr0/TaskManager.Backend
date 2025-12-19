namespace TaskManager.Web.Boards;

public class AddBoardMemberRequest
{
  public const string Route = "/Boards/{BoardId:int}/Members";

  public int BoardId { get; set; }
  public string Email { get; set; } = string.Empty;
  public string Role { get; set; } = string.Empty;
}
