using TaskManager.Core.BoardAggregate;
using TaskManager.Core.UserAggregate;

namespace TaskManager.UseCases.Boards.UpdateMemberRole;

public record UpdateBoardMemberRoleCommand(BoardId BoardId, UserId UserId, BoardRole Role, UserId RequestingUserId)
  : ICommand<Result<BoardMemberRoleUpdatedDto>>;

public record BoardMemberRoleUpdatedDto(BoardId BoardId, UserId UserId, UserEmail Email, BoardRole Role);
