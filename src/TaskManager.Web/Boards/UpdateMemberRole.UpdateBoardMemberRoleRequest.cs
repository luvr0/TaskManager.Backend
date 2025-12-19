namespace TaskManager.Web.Boards;

public class UpdateBoardMemberRoleRequest
{
  public const string Route = "/Boards/{BoardId:int}/Members/{MemberId:int}";

  public int BoardId { get; set; }
  public int MemberId { get; set; }
  public string Role { get; set; } = string.Empty;
}
