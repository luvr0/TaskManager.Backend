using TaskManager.UseCases.Boards;

namespace TaskManager.UseCases.Cards.List;

public class ListCardHandler(IListCardQueryService query)
  : IQueryHandler<ListCardQuery, Result<PagedResult<CardDto>>>
{
  public async ValueTask<Result<PagedResult<CardDto>>> Handle(ListCardQuery request,
                                                              CancellationToken cancellationToken)
  {
    var result = await query.ListAsync(request.BoardId,
                                       request.ColumnId,
                                       request.Page ?? 1,
                                       request.PerPage ?? Constants.DEFAULT_PAGE_SIZE);

    return Result.Success(result);
  }
}
