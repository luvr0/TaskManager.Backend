using Ardalis.Result;
using Ardalis.Specification;
using TaskManager.Core.BoardAggregate;
using TaskManager.Core.UserAggregate;
using TaskManager.UseCases.Boards.RemoveMember;

namespace TaskManager.UnitTests.UseCases.Boards;

public class RemoveBoardMemberHandlerTests
{
  private readonly IRepository<Board> _repository = Substitute.For<IRepository<Board>>();

  [Fact]
  public async Task Handle_ReturnsNotFound_WhenBoardDoesNotExist()
  {
    var handler = new RemoveBoardMemberHandler(_repository);
    var command = new RemoveBoardMemberCommand(BoardId.From(1), UserId.From(2), UserId.From(1));

    var result = await handler.Handle(command, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.NotFound);
    await _repository.DidNotReceive().UpdateAsync(Arg.Any<Board>(), Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_ReturnsNotFound_WhenRequesterIsNotManager()
  {
    var board = new Board(BoardName.From("Alpha"), UserId.From(1));
    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Board>>(), Arg.Any<CancellationToken>())
      .Returns(board);

    var handler = new RemoveBoardMemberHandler(_repository);
    var command = new RemoveBoardMemberCommand(BoardId.From(1), UserId.From(2), UserId.From(99));

    var result = await handler.Handle(command, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.NotFound);
    await _repository.DidNotReceive().UpdateAsync(Arg.Any<Board>(), Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_ReturnsInvalid_WhenMemberMissing()
  {
    var board = new Board(BoardName.From("Alpha"), UserId.From(1));
    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Board>>(), Arg.Any<CancellationToken>())
      .Returns(board);

    var handler = new RemoveBoardMemberHandler(_repository);
    var command = new RemoveBoardMemberCommand(BoardId.From(1), UserId.From(2), board.UserId);

    var result = await handler.Handle(command, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.Invalid);
    result.ValidationErrors.ShouldHaveSingleItem().Identifier.ShouldBe(nameof(command.UserId));
    await _repository.DidNotReceive().UpdateAsync(Arg.Any<Board>(), Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_RemovesMember_WhenAuthorized()
  {
    var board = new Board(BoardName.From("Alpha"), UserId.From(1));
    var memberId = UserId.From(2);
    board.AddMember(memberId, BoardRole.Reader);

    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Board>>(), Arg.Any<CancellationToken>())
      .Returns(board);

    var handler = new RemoveBoardMemberHandler(_repository);
    var command = new RemoveBoardMemberCommand(BoardId.From(1), memberId, board.UserId);

    var result = await handler.Handle(command, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.Ok);
    board.Members.ShouldNotContain(m => m.UserId == memberId);
    await _repository.Received(1).UpdateAsync(board, Arg.Any<CancellationToken>());
  }
}
