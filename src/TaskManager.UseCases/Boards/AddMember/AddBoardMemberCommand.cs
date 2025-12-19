using TaskManager.Core.BoardAggregate;
using TaskManager.Core.UserAggregate;

namespace TaskManager.UseCases.Boards.AddMember;

public record AddBoardMemberCommand(BoardId BoardId, UserEmail Email, BoardRole Role, UserId RequestingUserId)
  : ICommand<Result<BoardMemberAddedDto>>;

public record BoardMemberAddedDto(BoardId BoardId, UserId UserId, UserEmail Email, BoardRole Role);
