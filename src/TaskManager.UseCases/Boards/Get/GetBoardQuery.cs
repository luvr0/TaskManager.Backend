using TaskManager.Core.BoardAggregate;
using TaskManager.Core.UserAggregate;

namespace TaskManager.UseCases.Boards.Get;

public record GetBoardQuery(BoardId BoardId, UserId RequestingUserId) : IQuery<Result<BoardDto>>;
