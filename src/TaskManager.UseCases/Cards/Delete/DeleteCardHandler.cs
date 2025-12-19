using TaskManager.Core.BoardAggregate;
using TaskManager.Core.BoardAggregate.Specifications;

namespace TaskManager.UseCases.Cards.Delete;

public class DeleteCardHandler(IRepository<Board> repository) 
  : ICommandHandler<DeleteCardCommand, Result>
{
  public async ValueTask<Result> Handle(DeleteCardCommand request, CancellationToken cancellationToken)
  {
    var spec = new BoardByIdSpec(request.BoardId);
    var board = await repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (board == null) return Result.NotFound();

    if (!board.HasPermission(request.RequestingUserId, BoardPermission.Delete))
    {
      return Result.NotFound();
    }

    Card? card = null;
    Column? parentColumn = null;
    
    foreach (var column in board.Columns)
    {
      card = column.Cards.FirstOrDefault(c => c.Id == request.CardId);
      if (card != null)
      {
        parentColumn = column;
        break;
      }
    }
    
    if (card == null || parentColumn == null) return Result.NotFound();

    parentColumn.RemoveCard(card);
    
    await repository.UpdateAsync(board, cancellationToken);
    return Result.Success();
  }
}
