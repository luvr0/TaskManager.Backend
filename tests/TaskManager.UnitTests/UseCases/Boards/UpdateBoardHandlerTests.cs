using Ardalis.Result;
using TaskManager.Core.BoardAggregate;
using TaskManager.Core.UserAggregate;
using TaskManager.UnitTests.TestUtils;
using TaskManager.UseCases.Boards.Update;

namespace TaskManager.UnitTests.UseCases.Boards;

public class UpdateBoardHandlerTests
{
  private readonly IRepository<Board> _repository = Substitute.For<IRepository<Board>>();

  [Fact]
  public async Task Handle_ReturnsNotFound_WhenUserIsNotAuthorized()
  {
    var ownerId = UserId.From(1);
    var board = new Board(BoardName.From("Alpha"), ownerId).WithEntityId(BoardId.From(100));
    board.AddMember(UserId.From(2), BoardRole.Reader);

    _repository.GetByIdAsync(board.Id, Arg.Any<CancellationToken>()).Returns(board);

    var handler = new UpdateBoardHandler(_repository);
    var command = new UpdateBoardCommand(board.Id, BoardName.From("Beta"), UserId.From(2));

    var result = await handler.Handle(command, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.NotFound);
    await _repository.DidNotReceive().UpdateAsync(board, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_UpdatesBoard_WhenUserIsOwner()
  {
    var ownerId = UserId.From(1);
    var board = new Board(BoardName.From("Alpha"), ownerId).WithEntityId(BoardId.From(101));

    _repository.GetByIdAsync(board.Id, Arg.Any<CancellationToken>()).Returns(board);

    var handler = new UpdateBoardHandler(_repository);
    var command = new UpdateBoardCommand(board.Id, BoardName.From("Beta"), ownerId);

    var result = await handler.Handle(command, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.Ok);
    board.Name.Value.ShouldBe("Beta");
    await _repository.Received(1).UpdateAsync(board, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_UpdatesBoard_WhenUserIsManager()
  {
    var ownerId = UserId.From(1);
    var managerId = UserId.From(3);
    var board = new Board(BoardName.From("Alpha"), ownerId).WithEntityId(BoardId.From(102));
    board.AddMember(managerId, BoardRole.Manager);

    _repository.GetByIdAsync(board.Id, Arg.Any<CancellationToken>()).Returns(board);

    var handler = new UpdateBoardHandler(_repository);
    var command = new UpdateBoardCommand(board.Id, BoardName.From("Beta"), managerId);

    var result = await handler.Handle(command, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.Ok);
    board.Name.Value.ShouldBe("Beta");
    await _repository.Received(1).UpdateAsync(board, Arg.Any<CancellationToken>());
  }
}
