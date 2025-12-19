namespace TaskManager.Web.Cards;

public class CreateCardResponse(int id, string title, string description, int columnId, int boardId)
{
  public int Id { get; set; } = id;
  public string Title { get; set; } = title;
  public string Description { get; set; } = description;
  public int ColumnId { get; set; } = columnId;
  public int BoardId { get; set; } = boardId;
}
