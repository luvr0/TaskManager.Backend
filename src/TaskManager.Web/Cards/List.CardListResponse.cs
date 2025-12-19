namespace TaskManager.Web.Cards;

public record CardListResponse : UseCases.PagedResult<CardRecord>
{
  public CardListResponse(
    IReadOnlyList<CardRecord> Items,
    int Page,
    int PerPage,
    int TotalCount,
    int TotalPages)
    : base(Items, Page, PerPage, TotalCount, TotalPages)
  {
  }
}
