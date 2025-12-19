using Ardalis.Result;
using Ardalis.Specification;
using System.Linq;
using System.Reflection;
using System.Threading;
using TaskManager.Core.BoardAggregate;
using TaskManager.Core.UserAggregate;
using TaskManager.UseCases.Boards.UpdateMemberRole;

namespace TaskManager.UnitTests.UseCases.Boards;

public class UpdateBoardMemberRoleHandlerTests
{
  private readonly IRepository<Board> _boardRepository = Substitute.For<IRepository<Board>>();
  private readonly IReadRepository<User> _userRepository = Substitute.For<IReadRepository<User>>();

  [Fact]
  public async Task Handle_ReturnsNotFound_WhenBoardDoesNotExist()
  {
    var handler = new UpdateBoardMemberRoleHandler(_boardRepository, _userRepository);
    var command = new UpdateBoardMemberRoleCommand(BoardId.From(1), UserId.From(2), BoardRole.Editor, UserId.From(1));

    var result = await handler.Handle(command, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.NotFound);
    await _boardRepository.DidNotReceive().UpdateAsync(Arg.Any<Board>(), Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_ReturnsNotFound_WhenRequesterIsNotManager()
  {
    var board = new Board(BoardName.From("Launch"), UserId.From(1));
    var memberId = UserId.From(5);
    board.AddMember(memberId, BoardRole.Reader);

    _boardRepository.FirstOrDefaultAsync(Arg.Any<ISpecification<Board>>(), Arg.Any<CancellationToken>())
      .Returns(board);

    var handler = new UpdateBoardMemberRoleHandler(_boardRepository, _userRepository);
    var command = new UpdateBoardMemberRoleCommand(BoardId.From(2), memberId, BoardRole.Editor, UserId.From(99));

    var result = await handler.Handle(command, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.NotFound);
    await _boardRepository.DidNotReceive().UpdateAsync(Arg.Any<Board>(), Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_ReturnsInvalid_WhenMemberNotOnBoard()
  {
    var board = new Board(BoardName.From("Launch"), UserId.From(1));
    _boardRepository.FirstOrDefaultAsync(Arg.Any<ISpecification<Board>>(), Arg.Any<CancellationToken>())
      .Returns(board);

    var handler = new UpdateBoardMemberRoleHandler(_boardRepository, _userRepository);
    var command = new UpdateBoardMemberRoleCommand(BoardId.From(2), UserId.From(99), BoardRole.Editor, board.UserId);

    var result = await handler.Handle(command, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.Invalid);
    result.ValidationErrors.ShouldHaveSingleItem().Identifier.ShouldBe(nameof(command.UserId));
    await _boardRepository.DidNotReceive().UpdateAsync(Arg.Any<Board>(), Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_ReturnsInvalid_WhenUserEntityMissing()
  {
    var board = new Board(BoardName.From("Launch"), UserId.From(1));
    var memberId = UserId.From(5);
    board.AddMember(memberId, BoardRole.Reader);

    _boardRepository.FirstOrDefaultAsync(Arg.Any<ISpecification<Board>>(), Arg.Any<CancellationToken>())
      .Returns(board);
    _userRepository.FirstOrDefaultAsync(Arg.Any<ISpecification<User>>(), Arg.Any<CancellationToken>())
      .Returns((User?)null);

    var handler = new UpdateBoardMemberRoleHandler(_boardRepository, _userRepository);
    var command = new UpdateBoardMemberRoleCommand(BoardId.From(2), memberId, BoardRole.Editor, board.UserId);

    var result = await handler.Handle(command, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.Invalid);
    result.ValidationErrors.ShouldHaveSingleItem().Identifier.ShouldBe(nameof(command.UserId));
    await _boardRepository.DidNotReceive().UpdateAsync(Arg.Any<Board>(), Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_UpdatesMemberRole_WhenInputsValid()
  {
    var board = new Board(BoardName.From("Launch"), UserId.From(1));
    var member = CreateUser("member@gmail.com", 10);
    board.AddMember(member.Id, BoardRole.Reader);

    _boardRepository.FirstOrDefaultAsync(Arg.Any<ISpecification<Board>>(), Arg.Any<CancellationToken>())
      .Returns(board);
    _userRepository.FirstOrDefaultAsync(Arg.Any<ISpecification<User>>(), Arg.Any<CancellationToken>())
      .Returns(member);

    var handler = new UpdateBoardMemberRoleHandler(_boardRepository, _userRepository);
    var command = new UpdateBoardMemberRoleCommand(BoardId.From(3), member.Id, BoardRole.Manager, board.UserId);

    var result = await handler.Handle(command, CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
    board.Members.First(m => m.UserId == member.Id).Role.ShouldBe(BoardRole.Manager);
    await _boardRepository.Received(1).UpdateAsync(board, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_ReturnsSuccessWithoutPersist_WhenRoleUnchanged()
  {
    var board = new Board(BoardName.From("Launch"), UserId.From(1));
    var member = CreateUser("member@gmail.com", 11);
    board.AddMember(member.Id, BoardRole.Editor);

    _boardRepository.FirstOrDefaultAsync(Arg.Any<ISpecification<Board>>(), Arg.Any<CancellationToken>())
      .Returns(board);
    _userRepository.FirstOrDefaultAsync(Arg.Any<ISpecification<User>>(), Arg.Any<CancellationToken>())
      .Returns(member);

    var handler = new UpdateBoardMemberRoleHandler(_boardRepository, _userRepository);
    var command = new UpdateBoardMemberRoleCommand(BoardId.From(3), member.Id, BoardRole.Editor, board.UserId);

    var result = await handler.Handle(command, CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
    await _boardRepository.DidNotReceive().UpdateAsync(Arg.Any<Board>(), Arg.Any<CancellationToken>());
  }

  private static User CreateUser(string email, int id)
  {
    var user = new User(
      UserName.From("Member"),
      UserEmail.From(email),
      UserPassword.From("hashed"));

    var idProp = typeof(User).GetProperty("Id");
    if (idProp != null && idProp.CanWrite)
    {
      idProp.SetValue(user, UserId.From(id));
    }
    else
    {
      var field = typeof(User).GetField("<Id>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
      if (field != null) field.SetValue(user, UserId.From(id));
    }

    return user;
  }
}
