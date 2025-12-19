namespace TaskManager.Web.Boards;

public class BoardResponse(int id, string name, int ownerId, List<MemberResponse> members, List<ColumnResponse> columns)
{
  public int Id { get; set; } = id;
  public string Name { get; set; } = name;
  public int OwnerId { get; set; } = ownerId;
  public List<MemberResponse> Members { get; set; } = members;
  public List<ColumnResponse> Columns { get; set; } = columns;
}

public class ColumnResponse(int id, string name, int order, List<CardResponse> cards)
{
  public int Id { get; set; } = id;
  public string Name { get; set; } = name;
  public int Order { get; set; } = order;
  public List<CardResponse> Cards { get; set; } = cards;
}

public class CardResponse(int id, string title, string description, int order, string status)
{
  public int Id { get; set; } = id;
  public string Title { get; set; } = title;
  public string Description { get; set; } = description;
  public int Order { get; set; } = order;
  public string Status { get; set; } = status;
}

public class MemberResponse(int id, string name)
{
  public int Id { get; set; } = id;
  public string Name { get; set; } = name;
}
