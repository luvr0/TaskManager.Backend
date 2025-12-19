using Ardalis.Result;
using Ardalis.Specification;
using TaskManager.Core.BoardAggregate;
using TaskManager.Core.BoardAggregate.Specifications;
using TaskManager.Core.UserAggregate;
using TaskManager.UnitTests.TestUtils;
using TaskManager.UseCases.Columns.Delete;

namespace TaskManager.UnitTests.UseCases.Columns;

public class DeleteColumnHandlerTests
{
  private readonly IRepository<Board> _repository = Substitute.For<IRepository<Board>>();

  [Fact]
  public async Task Handle_ReturnsNotFound_WhenUserCannotDelete()
  {
    var ownerId = UserId.From(1);
    var readerId = UserId.From(2);
    var (board, column) = CreateBoardWithColumn(BoardId.From(20), ownerId, ColumnId.From(200));
    board.AddMember(readerId, BoardRole.Reader);

    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Board>>(), Arg.Any<CancellationToken>())
      .Returns(board);

    var handler = new DeleteColumnHandler(_repository);
    var command = new DeleteColumnCommand(column.Id, board.Id, readerId);

    var result = await handler.Handle(command, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.NotFound);
    board.Columns.ShouldContain(column);
    await _repository.DidNotReceive().UpdateAsync(board, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_RemovesColumn_WhenUserHasDeletePermission()
  {
    var ownerId = UserId.From(1);
    var managerId = UserId.From(4);
    var (board, column) = CreateBoardWithColumn(BoardId.From(21), ownerId, ColumnId.From(201));
    board.AddMember(managerId, BoardRole.Manager);

    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Board>>(), Arg.Any<CancellationToken>())
      .Returns(board);

    var handler = new DeleteColumnHandler(_repository);
    var command = new DeleteColumnCommand(column.Id, board.Id, managerId);

    var result = await handler.Handle(command, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.Ok);
    board.Columns.ShouldNotContain(column);
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
