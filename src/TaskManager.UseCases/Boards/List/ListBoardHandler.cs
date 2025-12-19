namespace TaskManager.UseCases.Boards.List;

public class ListBoardHandler(IListBoardQueryService query)
  : IQueryHandler<ListBoardQuery, Result<PagedResult<BoardDto>>>
{
  public async ValueTask<Result<PagedResult<BoardDto>>> Handle(ListBoardQuery request,
                                                               CancellationToken cancellationToken)
  {
    var result = await query.ListAsync(request.UserId, request.Page ?? 1, request.PerPage ?? Constants.DEFAULT_PAGE_SIZE);

    return Result.Success(result);
  }
}
