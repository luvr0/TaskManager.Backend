using TaskManager.Core.BoardAggregate;
using TaskManager.Core.BoardAggregate.Specifications;
using TaskManager.UseCases.Boards;

namespace TaskManager.UseCases.Cards.Update;

public class UpdateCardHandler(IRepository<Board> repository)
  : ICommandHandler<UpdateCardCommand, Result<CardDto>>
{
  public async ValueTask<Result<CardDto>> Handle(UpdateCardCommand command, CancellationToken cancellationToken)
  {
    var spec = new BoardByIdSpec(command.BoardId);
    var board = await repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (board == null) return Result.NotFound();

    if (!board.HasPermission(command.RequestingUserId, BoardPermission.Update))
    {
      return Result.NotFound();
    }

    Card? card = null;
    foreach (var column in board.Columns)
    {
      card = column.Cards.FirstOrDefault(c => c.Id == command.CardId);
      if (card != null) break;
    }
    
    if (card == null) return Result.NotFound();

    if (command.NewTitle != null)
      card.UpdateTitle(command.NewTitle.Value);
    
    if (command.NewDescription != null)
      card.UpdateDescription(command.NewDescription.Value);
    
    if (command.NewStatus != null)
      card.UpdateStatus(command.NewStatus);

    await repository.UpdateAsync(board, cancellationToken);

    return card.ToDto();
  }
}
