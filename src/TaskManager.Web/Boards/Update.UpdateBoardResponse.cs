namespace TaskManager.Web.Boards;

public class UpdateBoardResponse(BoardRecord board)
{
  public BoardRecord Board { get; set; } = board;
}
