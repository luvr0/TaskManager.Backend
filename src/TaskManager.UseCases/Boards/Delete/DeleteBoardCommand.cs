using TaskManager.Core.BoardAggregate;
using TaskManager.Core.UserAggregate;

namespace TaskManager.UseCases.Boards.Delete;

public record DeleteBoardCommand(BoardId BoardId, UserId RequestingUserId) : ICommand<Result>;
