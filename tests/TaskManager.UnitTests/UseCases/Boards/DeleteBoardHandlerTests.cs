using Ardalis.Result;
using Ardalis.Specification;
using Shouldly;
using TaskManager.Core.BoardAggregate;
using TaskManager.Core.UserAggregate;
using TaskManager.UseCases.Boards.Delete;

namespace TaskManager.UnitTests.UseCases.Boards;

public class DeleteBoardHandlerTests
{
  private readonly IRepository<Board> _repository = Substitute.For<IRepository<Board>>();

  [Fact]
  public async Task Handle_ReturnsNotFound_WhenBoardDoesNotExist()
  {
    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Board>>(), Arg.Any<CancellationToken>())
      .Returns((Board?)null);

    var handler = new DeleteBoardHandler(_repository);
    var command = new DeleteBoardCommand(BoardId.From(1), UserId.From(99));

    var result = await handler.Handle(command, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.NotFound);
    await _repository.DidNotReceive().DeleteAsync(Arg.Any<Board>(), Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_ReturnsNotFound_WhenUserLacksPermission()
  {
    var ownerId = UserId.From(1);
    var board = new Board(BoardName.From("Launch"), ownerId);
    var readerId = UserId.From(2);
    board.AddMember(readerId, BoardRole.Reader);

    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Board>>(), Arg.Any<CancellationToken>())
      .Returns(board);

    var handler = new DeleteBoardHandler(_repository);
    var command = new DeleteBoardCommand(BoardId.From(10), readerId);

    var result = await handler.Handle(command, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.NotFound);
    await _repository.DidNotReceive().DeleteAsync(Arg.Any<Board>(), Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_DeletesBoard_WhenUserHasPermission()
  {
    var ownerId = UserId.From(1);
    var board = new Board(BoardName.From("Launch"), ownerId);

    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Board>>(), Arg.Any<CancellationToken>())
      .Returns(board);

    var handler = new DeleteBoardHandler(_repository);
    var command = new DeleteBoardCommand(BoardId.From(10), ownerId);

    var result = await handler.Handle(command, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.Ok);
    await _repository.Received(1).DeleteAsync(board, Arg.Any<CancellationToken>());
  }
}
