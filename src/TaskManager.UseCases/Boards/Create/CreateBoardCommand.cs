using TaskManager.Core.BoardAggregate;
using TaskManager.Core.UserAggregate;

namespace TaskManager.UseCases.Boards.Create;

public record CreateBoardCommand(BoardName Name, UserId OwnerId) : ICommand<Result<BoardId>>;
