using Ardalis.Result;
using Ardalis.Specification;
using TaskManager.Core.BoardAggregate;
using TaskManager.Core.BoardAggregate.Specifications;
using TaskManager.Core.UserAggregate;
using TaskManager.UnitTests.TestUtils;
using TaskManager.UseCases.Columns.Update;

namespace TaskManager.UnitTests.UseCases.Columns;

public class UpdateColumnHandlerTests
{
  private readonly IRepository<Board> _repository = Substitute.For<IRepository<Board>>();

  [Fact]
  public async Task Handle_ReturnsNotFound_WhenUserLacksPermission()
  {
    var ownerId = UserId.From(1);
    var readerId = UserId.From(2);
    var (board, column) = CreateBoardWithColumn(BoardId.From(10), ownerId, ColumnId.From(100));
    board.AddMember(readerId, BoardRole.Reader);

    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Board>>(), Arg.Any<CancellationToken>())
      .Returns(board);

    var handler = new UpdateColumnHandler(_repository);
    var command = new UpdateColumnCommand(column.Id, board.Id, ColumnName.From("Doing"), readerId);

    var result = await handler.Handle(command, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.NotFound);
    column.Name.Value.ShouldBe("Backlog");
    await _repository.DidNotReceive().UpdateAsync(board, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_UpdatesColumn_WhenUserHasPermission()
  {
    var ownerId = UserId.From(1);
    var managerId = UserId.From(3);
    var (board, column) = CreateBoardWithColumn(BoardId.From(11), ownerId, ColumnId.From(101));
    board.AddMember(managerId, BoardRole.Manager);

    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Board>>(), Arg.Any<CancellationToken>())
      .Returns(board);

    var handler = new UpdateColumnHandler(_repository);
    var command = new UpdateColumnCommand(column.Id, board.Id, ColumnName.From("Doing"), managerId);

    var result = await handler.Handle(command, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.Ok);
    column.Name.Value.ShouldBe("Doing");
    await _repository.Received(1).UpdateAsync(board, Arg.Any<CancellationToken>());
  }

  private static (Board board, Column column) CreateBoardWithColumn(BoardId boardId, UserId ownerId, ColumnId columnId)
  {
    var board = new Board(BoardName.From("Alpha"), ownerId).WithEntityId(boardId);
    var column = new Column(ColumnName.From("Backlog"), boardId).WithEntityId(columnId);
    board.AddColumn(column);
    return (board, column);
  }
}
