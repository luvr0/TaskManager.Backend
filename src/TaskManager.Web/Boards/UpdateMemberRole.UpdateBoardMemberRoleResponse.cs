namespace TaskManager.Web.Boards;

public class UpdateBoardMemberRoleResponse(int boardId, BoardMemberRecord member)
{
  public int BoardId { get; set; } = boardId;
  public BoardMemberRecord Member { get; set; } = member;
}
