using TaskManager.Core.BoardAggregate;
using TaskManager.Core.UserAggregate;
using TaskManager.UseCases.Boards;

namespace TaskManager.UseCases.Cards.Update;

public record UpdateCardCommand(
  CardId CardId,
  BoardId BoardId,
  CardTitle? NewTitle,
  CardDescription? NewDescription,
  CardStatus? NewStatus,
  UserId RequestingUserId) : ICommand<Result<CardDto>>;
