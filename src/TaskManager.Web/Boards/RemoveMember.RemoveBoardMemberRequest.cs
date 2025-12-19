namespace TaskManager.Web.Boards;

public class RemoveBoardMemberRequest
{
  public const string Route = "/Boards/{BoardId:int}/Members/{MemberId:int}";

  public int BoardId { get; set; }
  public int MemberId { get; set; }
}
