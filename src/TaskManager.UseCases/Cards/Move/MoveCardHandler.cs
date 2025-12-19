using TaskManager.Core.BoardAggregate;
using TaskManager.Core.BoardAggregate.Specifications;
using TaskManager.UseCases.Boards;

namespace TaskManager.UseCases.Cards.Move;

public class MoveCardHandler(IRepository<Board> repository)
  : ICommandHandler<MoveCardCommand, Result<CardDto>>
{
  public async ValueTask<Result<CardDto>> Handle(MoveCardCommand command, CancellationToken cancellationToken)
  {
    var spec = new BoardByIdSpec(command.BoardId);
    var board = await repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (board == null) return Result.NotFound();

    if (!board.HasPermission(command.RequestingUserId, BoardPermission.Update))
    {
      return Result.NotFound();
    }

    // Find the card and its current column
    Card? card = null;
    Column? sourceColumn = null;
    
    foreach (var column in board.Columns)
    {
      card = column.Cards.FirstOrDefault(c => c.Id == command.CardId);
      if (card != null)
      {
        sourceColumn = column;
        break;
      }
    }
    
    if (card == null || sourceColumn == null) return Result.NotFound();

    // Find target column
    var targetColumn = board.Columns.FirstOrDefault(c => c.Id == command.TargetColumnId);
    if (targetColumn == null) return Result.NotFound();

    // Don't do anything if moving to the same column
    if (sourceColumn.Id == targetColumn.Id)
      return card.ToDto();

    // Remove from source and add to target
    sourceColumn.RemoveCard(card);
    targetColumn.AddCard(card);

    await repository.UpdateAsync(board, cancellationToken);

    return card.ToDto();
  }
}
