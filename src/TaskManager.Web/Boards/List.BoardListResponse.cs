namespace TaskManager.Web.Boards;

public record BoardListResponse : UseCases.PagedResult<BoardRecord>
{
  public BoardListResponse(
    IReadOnlyList<BoardRecord> Items,
    int Page,
    int PerPage,
    int TotalCount,
    int TotalPages)
    : base(Items, Page, PerPage, TotalCount, TotalPages)
  {
  }
}
