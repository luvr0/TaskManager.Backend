namespace TaskManager.Web.Cards;

public record CardRecord(int Id, string Title, string Description, int ColumnId, int Order, string Status);
