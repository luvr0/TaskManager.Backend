using TaskManager.Core.BoardAggregate;
using TaskManager.Core.UserAggregate;

namespace TaskManager.UseCases.Cards.Create;

public record CreateCardCommand(
  CardTitle Title,
  CardDescription? Description,
  ColumnId ColumnId,
  BoardId BoardId,
  UserId RequestingUserId) : ICommand<Result<CardId>>;
