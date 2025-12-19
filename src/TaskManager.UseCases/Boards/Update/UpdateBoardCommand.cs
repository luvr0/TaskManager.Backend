using TaskManager.Core.BoardAggregate;
using TaskManager.Core.UserAggregate;

namespace TaskManager.UseCases.Boards.Update;

public record UpdateBoardCommand(BoardId BoardId, BoardName NewName, UserId RequestingUserId) : ICommand<Result<BoardDto>>;
