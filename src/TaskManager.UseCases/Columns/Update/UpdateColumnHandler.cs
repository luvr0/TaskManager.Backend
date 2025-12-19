using TaskManager.Core.BoardAggregate;
using TaskManager.Core.BoardAggregate.Specifications;
using TaskManager.UseCases.Boards;

namespace TaskManager.UseCases.Columns.Update;

public class UpdateColumnHandler(IRepository<Board> repository)
  : ICommandHandler<UpdateColumnCommand, Result<ColumnDto>>
{
  public async ValueTask<Result<ColumnDto>> Handle(UpdateColumnCommand command, CancellationToken cancellationToken)
  {
    var spec = new BoardByIdSpec(command.BoardId);
    var board = await repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (board == null) return Result.NotFound();

    if (!board.HasPermission(command.RequestingUserId, BoardPermission.Update))
    {
      return Result.NotFound();
    }

    var column = board.Columns.FirstOrDefault(c => c.Id == command.ColumnId);
    if (column == null) return Result.NotFound();

    column.UpdateName(command.NewName);
    await repository.UpdateAsync(board, cancellationToken);

    return column.ToDto();
  }
}
