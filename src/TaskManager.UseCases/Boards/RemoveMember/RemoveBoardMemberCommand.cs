using TaskManager.Core.BoardAggregate;
using TaskManager.Core.UserAggregate;

namespace TaskManager.UseCases.Boards.RemoveMember;

public record RemoveBoardMemberCommand(BoardId BoardId, UserId UserId, UserId RequestingUserId) : ICommand<Result>;
