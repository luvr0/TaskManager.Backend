using TaskManager.Core.BoardAggregate;
using TaskManager.Core.UserAggregate;
using TaskManager.UseCases.Boards;

namespace TaskManager.UseCases.Cards.Move;

public record MoveCardCommand(CardId CardId, BoardId BoardId, ColumnId TargetColumnId, UserId RequestingUserId) : ICommand<Result<CardDto>>;
