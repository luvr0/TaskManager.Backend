using TaskManager.Core.BoardAggregate;
using TaskManager.Core.BoardAggregate.Specifications;
using TaskManager.UseCases.Boards;

namespace TaskManager.UseCases.Columns.Get;

public class GetColumnHandler(IReadRepository<Board> repository)
  : IQueryHandler<GetColumnQuery, Result<ColumnDto>>
{
  public async ValueTask<Result<ColumnDto>> Handle(GetColumnQuery request, 
                                                    CancellationToken cancellationToken)
  {
    var spec = new BoardByIdSpec(request.BoardId);
    var board = await repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (board == null) return Result.NotFound();

    var column = board.Columns.FirstOrDefault(c => c.Id == request.ColumnId);
    if (column == null) return Result.NotFound();

    return column.ToDto();
  }
}
