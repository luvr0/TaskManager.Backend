namespace TaskManager.Web.Columns;

public record ColumnListResponse : UseCases.PagedResult<ColumnRecord>
{
  public ColumnListResponse(
    IReadOnlyList<ColumnRecord> Items,
    int Page,
    int PerPage,
    int TotalCount,
    int TotalPages)
    : base(Items, Page, PerPage, TotalCount, TotalPages)
  {
  }
}
