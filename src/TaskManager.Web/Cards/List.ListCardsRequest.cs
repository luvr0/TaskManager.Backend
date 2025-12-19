namespace TaskManager.Web.Cards;

public sealed class ListCardsRequest
{
  public const string Route = "/Boards/{BoardId}/Columns/{ColumnId}/Cards";

  [BindFrom("BoardId")]
  public int BoardId { get; init; }

  [BindFrom("ColumnId")]
  public int ColumnId { get; init; }

  [BindFrom("page")]
  public int Page { get; init; } = 1;

  [BindFrom("per_page")]
  public int PerPage { get; init; } = UseCases.Constants.DEFAULT_PAGE_SIZE;
}
