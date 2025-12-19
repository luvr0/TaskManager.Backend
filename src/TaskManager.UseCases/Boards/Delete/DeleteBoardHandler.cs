using TaskManager.Core.BoardAggregate;
using TaskManager.Core.BoardAggregate.Specifications;

namespace TaskManager.UseCases.Boards.Delete;

public class DeleteBoardHandler(IRepository<Board> repository) : ICommandHandler<DeleteBoardCommand, Result>
{
  public async ValueTask<Result> Handle(DeleteBoardCommand request, CancellationToken cancellationToken)
  {
    var board = await repository.FirstOrDefaultAsync(new BoardByIdSpec(request.BoardId), cancellationToken);
    if (board is null) return Result.NotFound();

    if (board.UserId != request.RequestingUserId)
    {
      var member = board.GetMember(request.RequestingUserId);
      if (member is null || !member.HasPermission(BoardPermission.Delete))
      {
        return Result.NotFound();
      }
    }

    await repository.DeleteAsync(board, cancellationToken);
    return Result.Success();
  }
}
