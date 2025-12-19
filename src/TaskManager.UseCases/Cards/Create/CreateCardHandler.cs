using TaskManager.Core.BoardAggregate;
using TaskManager.Core.BoardAggregate.Specifications;

namespace TaskManager.UseCases.Cards.Create;

public class CreateCardHandler(IRepository<Board> repository) 
  : ICommandHandler<CreateCardCommand, Result<CardId>>
{
  public async ValueTask<Result<CardId>> Handle(CreateCardCommand command, CancellationToken cancellationToken)
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

    var column = board.Columns.FirstOrDefault(c => c.Id == command.ColumnId);
    if (column == null) return Result.NotFound();

    var card = command.Description != null 
      ? column.AddCard(command.Title, command.Description.Value)
      : column.AddCard(command.Title, CardDescription.From(string.Empty));

    await repository.UpdateAsync(board, cancellationToken);

    return card.Id;
  }
}
