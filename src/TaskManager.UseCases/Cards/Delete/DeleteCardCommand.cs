using TaskManager.Core.BoardAggregate;
using TaskManager.Core.UserAggregate;

namespace TaskManager.UseCases.Cards.Delete;

public record DeleteCardCommand(CardId CardId, BoardId BoardId, UserId RequestingUserId) : ICommand<Result>;
