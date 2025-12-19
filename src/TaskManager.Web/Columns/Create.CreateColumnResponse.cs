namespace TaskManager.Web.Columns;

public class CreateColumnResponse(int id, string name, int boardId)
{
  public int Id { get; set; } = id;
  public string Name { get; set; } = name;
  public int BoardId { get; set; } = boardId;
}
