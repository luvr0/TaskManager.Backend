using TaskManager.Core.BoardAggregate;
using TaskManager.Core.BoardAggregate.Specifications;

namespace TaskManager.UseCases.Columns.Delete;

public class DeleteColumnHandler(IRepository<Board> repository) 
  : ICommandHandler<DeleteColumnCommand, Result>
{
  public async ValueTask<Result> Handle(DeleteColumnCommand request, CancellationToken cancellationToken)
  {
    var spec = new BoardByIdSpec(request.BoardId);
    var board = await repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (board == null) return Result.NotFound();

    if (!board.HasPermission(request.RequestingUserId, BoardPermission.Delete))
    {
      return Result.NotFound();
    }

    var column = board.Columns.FirstOrDefault(c => c.Id == request.ColumnId);
    if (column == null) return Result.NotFound();

    board.RemoveColumn(column);
    
    await repository.UpdateAsync(board, cancellationToken);
    return Result.Success();
  }
}
