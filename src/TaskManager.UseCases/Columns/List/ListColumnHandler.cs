using TaskManager.UseCases.Boards;

namespace TaskManager.UseCases.Columns.List;

public class ListColumnHandler(IListColumnQueryService query)
  : IQueryHandler<ListColumnQuery, Result<PagedResult<ColumnDto>>>
{
  public async ValueTask<Result<PagedResult<ColumnDto>>> Handle(ListColumnQuery request,
                                                                CancellationToken cancellationToken)
  {
    var result = await query.ListAsync(request.BoardId,
                                       request.Page ?? 1,
                                       request.PerPage ?? Constants.DEFAULT_PAGE_SIZE);

    return Result.Success(result);
  }
}
