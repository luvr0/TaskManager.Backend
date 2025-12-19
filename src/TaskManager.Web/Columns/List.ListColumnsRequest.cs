namespace TaskManager.Web.Columns;

public sealed class ListColumnsRequest
{
  public const string Route = "/Boards/{BoardId}/Columns";

  [BindFrom("BoardId")]
  public int BoardId { get; init; }

  [BindFrom("page")]
  public int Page { get; init; } = 1;

  [BindFrom("per_page")]
  public int PerPage { get; init; } = UseCases.Constants.DEFAULT_PAGE_SIZE;
}
