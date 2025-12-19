using Ardalis.Result;
using Ardalis.Specification;
using TaskManager.Core.BoardAggregate;
using TaskManager.Core.UserAggregate;
using TaskManager.UseCases.Boards.Get;
using TaskManager.UnitTests.TestUtils;

namespace TaskManager.UnitTests.UseCases.Boards;

public class GetBoardHandlerTests
{
  private readonly IReadRepository<Board> _repository = Substitute.For<IReadRepository<Board>>();

  public static IEnumerable<object[]> MemberRolesWithReadPermission => new[]
  {
    new object[] { BoardRole.Reader },
    new object[] { BoardRole.Editor },
    new object[] { BoardRole.Manager }
  };

  [Theory]
  [MemberData(nameof(MemberRolesWithReadPermission))]
  public async Task Handle_AllowsMembersToReadBoards(BoardRole role)
  {
    var board = CreateBoard(BoardId.From(10), UserId.From(1));
    var memberId = UserId.From(42);
    board.AddMember(memberId, role);

    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Board>>(), Arg.Any<CancellationToken>())
      .Returns(board);

    var handler = new GetBoardHandler(_repository);
    var query = new GetBoardQuery(board.Id, memberId);

    var result = await handler.Handle(query, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.Ok);
  }

  [Fact]
  public async Task Handle_ReturnsNotFound_ForUsersOutsideTheBoard()
  {
    var board = CreateBoard(BoardId.From(20), UserId.From(5));

    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Board>>(), Arg.Any<CancellationToken>())
      .Returns(board);

    var handler = new GetBoardHandler(_repository);
    var query = new GetBoardQuery(board.Id, UserId.From(99));

    var result = await handler.Handle(query, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.NotFound);
  }

  private static Board CreateBoard(BoardId boardId, UserId ownerId)
  {
    var board = new Board(BoardName.From("Alpha"), ownerId);
    board.WithEntityId(boardId);
    return board;
  }
}
