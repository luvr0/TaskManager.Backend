using System.Linq;
using TaskManager.Core.BoardAggregate;
using TaskManager.Core.BoardAggregate.Specifications;
using TaskManager.Core.UserAggregate;

namespace TaskManager.UseCases.Boards.RemoveMember;

public class RemoveBoardMemberHandler(IRepository<Board> repository)
  : ICommandHandler<RemoveBoardMemberCommand, Result>
{
  public async ValueTask<Result> Handle(RemoveBoardMemberCommand command, CancellationToken cancellationToken)
  {
    var board = await repository.FirstOrDefaultAsync(new BoardByIdSpec(command.BoardId), cancellationToken);
    if (board is null) return Result.NotFound();

    if (!IsManager(board, command.RequestingUserId)) return Result.NotFound();

    var member = board.Members.FirstOrDefault(m => m.UserId == command.UserId);
    if (member is null)
    {
      return Result.Invalid(new[] { new ValidationError { Identifier = nameof(command.UserId), ErrorMessage = "Member not found on board." } });
    }

    board.RemoveMember(command.UserId);
    await repository.UpdateAsync(board, cancellationToken);

    return Result.Success();
  }

  private static bool IsManager(Board board, UserId userId)
    => board.UserId == userId || board.GetMember(userId)?.Role == BoardRole.Manager;
}
