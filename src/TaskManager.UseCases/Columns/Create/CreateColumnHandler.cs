using TaskManager.Core.BoardAggregate;
using TaskManager.Core.BoardAggregate.Specifications;

namespace TaskManager.UseCases.Columns.Create;

public class CreateColumnHandler(IRepository<Board> repository) 
  : ICommandHandler<CreateColumnCommand, Result<ColumnId>>
{
  public async ValueTask<Result<ColumnId>> Handle(CreateColumnCommand command, CancellationToken cancellationToken)
  {
    var spec = new BoardByIdSpec(command.BoardId);
    var board = await repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (board == null) return Result.NotFound();

    if (!board.CanBeAccessedBy(command.RequestingUserId)) return Result.NotFound();

    var isOwner = board.UserId == command.RequestingUserId;
    if (!isOwner)
    {
      var member = board.GetMember(command.RequestingUserId);
      if (member is null || !member.HasPermission(BoardPermission.Create)) return Result.NotFound();
    }

    var column = new Column(command.Name, command.BoardId);
    board.AddColumn(column);

    await repository.UpdateAsync(board, cancellationToken);

    return column.Id;
  }
}
