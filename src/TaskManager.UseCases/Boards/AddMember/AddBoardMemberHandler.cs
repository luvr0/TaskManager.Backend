using TaskManager.Core.BoardAggregate;
using TaskManager.Core.BoardAggregate.Specifications;
using TaskManager.Core.UserAggregate;
using TaskManager.Core.UserAggregate.Specifications;

namespace TaskManager.UseCases.Boards.AddMember;

public class AddBoardMemberHandler(
  IRepository<Board> boardRepository,
  IReadRepository<User> userRepository)
  : ICommandHandler<AddBoardMemberCommand, Result<BoardMemberAddedDto>>
{
  public async ValueTask<Result<BoardMemberAddedDto>> Handle(AddBoardMemberCommand command, CancellationToken cancellationToken)
  {
    var boardSpec = new BoardByIdSpec(command.BoardId);
    var board = await boardRepository.FirstOrDefaultAsync(boardSpec, cancellationToken);
    if (board is null)
    {
      return Result.NotFound();
    }

    if (!IsManager(board, command.RequestingUserId))
    {
      return Result.NotFound();
    }

    var user = await userRepository.FirstOrDefaultAsync(new UserByEmailSpec(command.Email), cancellationToken);
    if (user is null)
    {
      return Result<BoardMemberAddedDto>.Invalid(new[]
      {
        new ValidationError
        {
          Identifier = nameof(command.Email),
          ErrorMessage = "User with the provided email was not found."
        }
      });
    }

    if (board.Members.Any(member => member.UserId == user.Id))
    {
      return Result<BoardMemberAddedDto>.Invalid(new[]
      {
        new ValidationError
        {
          Identifier = nameof(command.Email),
          ErrorMessage = "User is already a member of this board."
        }
      });
    }

    board.AddMember(user.Id, command.Role);
    await boardRepository.UpdateAsync(board, cancellationToken);

    return Result.Success(new BoardMemberAddedDto(board.Id, user.Id, user.Email, command.Role));
  }

  private static bool IsManager(Board board, UserId userId)
    => board.UserId == userId || board.GetMember(userId)?.Role == BoardRole.Manager;
}
