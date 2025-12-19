using Ardalis.Result;
using Ardalis.Specification;
using TaskManager.Core.BoardAggregate;
using TaskManager.Core.UserAggregate;
using TaskManager.UseCases.Cards.Create;
using TaskManager.UnitTests.TestUtils;

namespace TaskManager.UnitTests.UseCases.Cards;

public class CreateCardHandlerTests
{
  private readonly IRepository<Board> _repository = Substitute.For<IRepository<Board>>();

  public static IEnumerable<object[]> RolesWithoutCreatePermission => new[]
  {
    new object[] { BoardRole.Reader },
    new object[] { BoardRole.Editor }
  };

  [Theory]
  [MemberData(nameof(RolesWithoutCreatePermission))]
  public async Task Handle_ReturnsNotFound_WhenRoleCannotCreate(BoardRole role)
  {
    var ownerId = UserId.From(1);
    var board = CreateBoardWithColumn(BoardId.From(10), ownerId, ColumnId.From(100));
    var requesterId = UserId.From(2);
    board.AddMember(requesterId, role);

    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Board>>(), Arg.Any<CancellationToken>())
      .Returns(board);

    var handler = new CreateCardHandler(_repository);
    var command = new CreateCardCommand(
      CardTitle.From("Task"),
      CardDescription.From("Details"),
      ColumnId.From(100),
      board.Id,
      requesterId);

    var result = await handler.Handle(command, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.NotFound);
    await _repository.DidNotReceive().UpdateAsync(Arg.Any<Board>(), Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_AllowsOwnerToCreateCards()
  {
    var ownerId = UserId.From(1);
    var columnId = ColumnId.From(200);
    var board = CreateBoardWithColumn(BoardId.From(20), ownerId, columnId);

    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Board>>(), Arg.Any<CancellationToken>())
      .Returns(board);

    var handler = new CreateCardHandler(_repository);
    var command = new CreateCardCommand(
      CardTitle.From("Fix bug"),
      CardDescription.From("details"),
      columnId,
      board.Id,
      ownerId);

    var result = await handler.Handle(command, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.Ok);
    board.Columns.Single().Cards.ShouldContain(c => c.Title == command.Title);
    await _repository.Received(1).UpdateAsync(board, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_AllowsManagersToCreateCards()
  {
    var ownerId = UserId.From(1);
    var columnId = ColumnId.From(300);
    var board = CreateBoardWithColumn(BoardId.From(30), ownerId, columnId);
    var managerId = UserId.From(9);
    board.AddMember(managerId, BoardRole.Manager);

    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Board>>(), Arg.Any<CancellationToken>())
      .Returns(board);

    var handler = new CreateCardHandler(_repository);
    var command = new CreateCardCommand(
      CardTitle.From("Deploy"),
      CardDescription.From("Deploy steps"),
      columnId,
      board.Id,
      managerId);

    var result = await handler.Handle(command, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.Ok);
    board.Columns.Single().Cards.ShouldContain(c => c.Title == command.Title);
    await _repository.Received(1).UpdateAsync(board, Arg.Any<CancellationToken>());
  }

  private static Board CreateBoardWithColumn(BoardId boardId, UserId ownerId, ColumnId columnId)
  {
    var board = new Board(BoardName.From("Alpha"), ownerId);
    board.WithEntityId(boardId);

    var column = new Column(ColumnName.From("Backlog"), boardId);
    column.WithEntityId(columnId);
    board.AddColumn(column);

    return board;
  }
}
