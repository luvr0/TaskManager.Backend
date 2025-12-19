using Ardalis.Result;
using Ardalis.Specification;
using System.Reflection;
using System.Threading;
using TaskManager.Core.BoardAggregate;
using TaskManager.Core.UserAggregate;
using TaskManager.UseCases.Boards.AddMember;

namespace TaskManager.UnitTests.UseCases.Boards;

public class AddBoardMemberHandlerTests
{
  private readonly IRepository<Board> _boardRepository = Substitute.For<IRepository<Board>>();
  private readonly IReadRepository<User> _userRepository = Substitute.For<IReadRepository<User>>();

  [Fact]
  public async Task Handle_ReturnsNotFound_WhenBoardDoesNotExist()
  {
    var handler = new AddBoardMemberHandler(_boardRepository, _userRepository);
    var command = new AddBoardMemberCommand(BoardId.From(1), UserEmail.From("member@gmail.com"), BoardRole.Reader, UserId.From(1));

    var result = await handler.Handle(command, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.NotFound);
    await _boardRepository.DidNotReceive().UpdateAsync(Arg.Any<Board>(), Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_ReturnsNotFound_WhenRequesterIsNotManager()
  {
    var board = new Board(BoardName.From("Product"), UserId.From(1));
    _boardRepository.FirstOrDefaultAsync(Arg.Any<ISpecification<Board>>(), Arg.Any<CancellationToken>())
      .Returns(board);

    var handler = new AddBoardMemberHandler(_boardRepository, _userRepository);
    var command = new AddBoardMemberCommand(BoardId.From(2), UserEmail.From("member@gmail.com"), BoardRole.Editor, UserId.From(99));

    var result = await handler.Handle(command, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.NotFound);
    await _boardRepository.DidNotReceive().UpdateAsync(Arg.Any<Board>(), Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_ReturnsInvalid_WhenUserDoesNotExist()
  {
    var board = new Board(BoardName.From("Product"), UserId.From(1));
    _boardRepository.FirstOrDefaultAsync(Arg.Any<ISpecification<Board>>(), Arg.Any<CancellationToken>())
      .Returns(board);

    var handler = new AddBoardMemberHandler(_boardRepository, _userRepository);
    var command = new AddBoardMemberCommand(BoardId.From(2), UserEmail.From("missing@gmail.com"), BoardRole.Editor, board.UserId);

    var result = await handler.Handle(command, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.Invalid);
    result.ValidationErrors.ShouldHaveSingleItem().Identifier.ShouldBe(nameof(command.Email));
    await _boardRepository.DidNotReceive().UpdateAsync(Arg.Any<Board>(), Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_ReturnsInvalid_WhenUserAlreadyMember()
  {
    var board = new Board(BoardName.From("Product"), UserId.From(1));
    var member = CreateUser("member@gmail.com", 42);
    board.AddMember(member.Id, BoardRole.Reader);

    _boardRepository.FirstOrDefaultAsync(Arg.Any<ISpecification<Board>>(), Arg.Any<CancellationToken>())
      .Returns(board);
    _userRepository.FirstOrDefaultAsync(Arg.Any<ISpecification<User>>(), Arg.Any<CancellationToken>())
      .Returns(member);

    var handler = new AddBoardMemberHandler(_boardRepository, _userRepository);
    var command = new AddBoardMemberCommand(BoardId.From(3), member.Email, BoardRole.Reader, board.UserId);

    var result = await handler.Handle(command, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.Invalid);
    result.ValidationErrors.ShouldHaveSingleItem().Identifier.ShouldBe(nameof(command.Email));
  }

  [Fact]
  public async Task Handle_AddsMemberAndPersistsBoard_WhenOwnerRequests()
  {
    var board = new Board(BoardName.From("Launch"), UserId.From(1));
    var member = CreateUser("member2@gmail.com", 43);

    _boardRepository.FirstOrDefaultAsync(Arg.Any<ISpecification<Board>>(), Arg.Any<CancellationToken>())
      .Returns(board);
    _userRepository.FirstOrDefaultAsync(Arg.Any<ISpecification<User>>(), Arg.Any<CancellationToken>())
      .Returns(member);

    var handler = new AddBoardMemberHandler(_boardRepository, _userRepository);
    var command = new AddBoardMemberCommand(BoardId.From(4), member.Email, BoardRole.Manager, board.UserId);

    var result = await handler.Handle(command, CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
    board.Members.ShouldContain(m => m.UserId == member.Id && m.Role == BoardRole.Manager);
    await _boardRepository.Received(1).UpdateAsync(board, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_AllowsManagerMemberToAdd()
  {
    var ownerId = UserId.From(1);
    var managerId = UserId.From(50);
    var board = new Board(BoardName.From("Launch"), ownerId);
    board.AddMember(managerId, BoardRole.Manager);
    var member = CreateUser("member3@gmail.com", 44);

    _boardRepository.FirstOrDefaultAsync(Arg.Any<ISpecification<Board>>(), Arg.Any<CancellationToken>())
      .Returns(board);
    _userRepository.FirstOrDefaultAsync(Arg.Any<ISpecification<User>>(), Arg.Any<CancellationToken>())
      .Returns(member);

    var handler = new AddBoardMemberHandler(_boardRepository, _userRepository);
    var command = new AddBoardMemberCommand(BoardId.From(5), member.Email, BoardRole.Reader, managerId);

    var result = await handler.Handle(command, CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
    board.Members.ShouldContain(m => m.UserId == member.Id && m.Role == BoardRole.Reader);
    await _boardRepository.Received(1).UpdateAsync(board, Arg.Any<CancellationToken>());
  }

  private static User CreateUser(string email, int id)
  {
    var user = new User(
      UserName.From("Member"),
      UserEmail.From(email),
      UserPassword.From("hashed"));

    // set the Id via reflection (Ardalis EntityBase.Id has protected setter)
    var idProp = typeof(User).GetProperty("Id");
    if (idProp != null && idProp.CanWrite)
    {
      idProp.SetValue(user, UserId.From(id));
    }
    else
    {
      // try backing field set for cases where property has protected setter
      var field = typeof(User).GetField("<Id>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
      if (field != null) field.SetValue(user, UserId.From(id));
    }

    return user;
  }
}
