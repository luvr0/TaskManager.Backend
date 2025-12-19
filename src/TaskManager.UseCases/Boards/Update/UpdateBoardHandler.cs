using TaskManager.Core.BoardAggregate;

namespace TaskManager.UseCases.Boards.Update;

public class UpdateBoardHandler(IRepository<Board> _repository)
  : ICommandHandler<UpdateBoardCommand, Result<BoardDto>>
{
  public async ValueTask<Result<BoardDto>> Handle(UpdateBoardCommand command, CancellationToken cancellationToken)
  {
    var existing = await _repository.GetByIdAsync(command.BoardId, cancellationToken);
    if (existing == null) return Result.NotFound();

    if (!existing.HasPermission(command.RequestingUserId, BoardPermission.Update))
    {
      return Result.NotFound();
    }

    existing.UpdateName(command.NewName);
    await _repository.UpdateAsync(existing, cancellationToken);

    return existing.ToDto();
  }
}
