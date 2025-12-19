using Ardalis.Result;
using Ardalis.Specification;
using TaskManager.Core.BoardAggregate;
using TaskManager.Core.UserAggregate;
using TaskManager.UseCases.Columns.Create;
using TaskManager.UnitTests.TestUtils;

namespace TaskManager.UnitTests.UseCases.Columns;

public class CreateColumnHandlerTests
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
    var board = CreateBoard(BoardId.From(1), ownerId);
    var memberId = UserId.From(2);
    board.AddMember(memberId, role);

    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Board>>(), Arg.Any<CancellationToken>())
      .Returns(board);

    var handler = new CreateColumnHandler(_repository);
    var command = new CreateColumnCommand(ColumnName.From("Backlog"), board.Id, memberId);

    var result = await handler.Handle(command, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.NotFound);
    await _repository.DidNotReceive().UpdateAsync(Arg.Any<Board>(), Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_AllowsOwnerToCreateColumns()
  {
    var ownerId = UserId.From(1);
    var board = CreateBoard(BoardId.From(2), ownerId);

    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Board>>(), Arg.Any<CancellationToken>())
      .Returns(board);

    var handler = new CreateColumnHandler(_repository);
    var command = new CreateColumnCommand(ColumnName.From("Doing"), board.Id, ownerId);

    var result = await handler.Handle(command, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.Ok);
    board.Columns.ShouldContain(c => c.Name == command.Name);
    await _repository.Received(1).UpdateAsync(board, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_AllowsManagersToCreateColumns()
  {
    var ownerId = UserId.From(1);
    var board = CreateBoard(BoardId.From(3), ownerId);
    var managerId = UserId.From(5);
    board.AddMember(managerId, BoardRole.Manager);

    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Board>>(), Arg.Any<CancellationToken>())
      .Returns(board);

    var handler = new CreateColumnHandler(_repository);
    var command = new CreateColumnCommand(ColumnName.From("Review"), board.Id, managerId);

    var result = await handler.Handle(command, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.Ok);
    board.Columns.ShouldContain(c => c.Name == command.Name);
    await _repository.Received(1).UpdateAsync(board, Arg.Any<CancellationToken>());
  }

  private static Board CreateBoard(BoardId boardId, UserId ownerId)
  {
    var board = new Board(BoardName.From("Alpha"), ownerId);
    board.WithEntityId(boardId);
    return board;
  }
}
