namespace TaskManager.Web.Boards;

public class AddBoardMemberResponse(int boardId, BoardMemberRecord member)
{
  public int BoardId { get; set; } = boardId;
  public BoardMemberRecord Member { get; set; } = member;
}

public record BoardMemberRecord(int UserId, string Email, string Role);
