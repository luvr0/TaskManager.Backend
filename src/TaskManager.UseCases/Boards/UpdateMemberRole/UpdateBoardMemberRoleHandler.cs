using System.Linq;
using TaskManager.Core.BoardAggregate;
using TaskManager.Core.BoardAggregate.Specifications;
using TaskManager.Core.UserAggregate;
using TaskManager.Core.UserAggregate.Specifications;

namespace TaskManager.UseCases.Boards.UpdateMemberRole;

public class UpdateBoardMemberRoleHandler(
  IRepository<Board> boardRepository,
  IReadRepository<User> userRepository)
  : ICommandHandler<UpdateBoardMemberRoleCommand, Result<BoardMemberRoleUpdatedDto>>
{
  public async ValueTask<Result<BoardMemberRoleUpdatedDto>> Handle(
    UpdateBoardMemberRoleCommand command,
    CancellationToken cancellationToken)
  {
    var board = await boardRepository.FirstOrDefaultAsync(new BoardByIdSpec(command.BoardId), cancellationToken);
    if (board is null)
    {
      return Result.NotFound();
    }

    if (!IsManager(board, command.RequestingUserId))
    {
      return Result.NotFound();
    }

    var member = board.Members.FirstOrDefault(m => m.UserId == command.UserId);
    if (member is null)
    {
      return Result<BoardMemberRoleUpdatedDto>.Invalid(new[]
      {
        new ValidationError
        {
          Identifier = nameof(command.UserId),
          ErrorMessage = "Member does not belong to this board."
        }
      });
    }

    var user = await userRepository.FirstOrDefaultAsync(new UserByIdSpec(command.UserId), cancellationToken);
    if (user is null)
    {
      return Result<BoardMemberRoleUpdatedDto>.Invalid(new[]
      {
        new ValidationError
        {
          Identifier = nameof(command.UserId),
          ErrorMessage = "User associated with this member could not be found."
        }
      });
    }

    if (member.Role == command.Role)
    {
      return Result.Success(new BoardMemberRoleUpdatedDto(board.Id, command.UserId, user.Email, member.Role));
    }

    board.UpdateMemberRole(command.UserId, command.Role);
    await boardRepository.UpdateAsync(board, cancellationToken);

    return Result.Success(new BoardMemberRoleUpdatedDto(board.Id, command.UserId, user.Email, command.Role));
  }

  private static bool IsManager(Board board, UserId userId)
    => board.UserId == userId || board.GetMember(userId)?.Role == BoardRole.Manager;
}
