using Ardalis.Result;
using Ardalis.Specification;
using TaskManager.Core.BoardAggregate;
using TaskManager.Core.BoardAggregate.Specifications;
using TaskManager.Core.UserAggregate;
using TaskManager.UnitTests.TestUtils;
using TaskManager.UseCases.Cards.Move;

namespace TaskManager.UnitTests.UseCases.Cards;

public class MoveCardHandlerTests
{
  private readonly IRepository<Board> _repository = Substitute.For<IRepository<Board>>();

  [Fact]
  public async Task Handle_ReturnsNotFound_WhenUserCannotUpdate()
  {
    var readerId = UserId.From(2);
    var setup = CreateBoardWithColumns();
    setup.Board.AddMember(readerId, BoardRole.Reader);

    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Board>>(), Arg.Any<CancellationToken>()).Returns(setup.Board);

    var handler = new MoveCardHandler(_repository);
    var command = new MoveCardCommand(setup.Card.Id, setup.Board.Id, setup.TargetColumn.Id, readerId);

    var result = await handler.Handle(command, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.NotFound);
    setup.SourceColumn.Cards.ShouldContain(setup.Card);
    setup.TargetColumn.Cards.ShouldBeEmpty();
    await _repository.DidNotReceive().UpdateAsync(setup.Board, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_MovesCard_WhenUserHasUpdatePermission()
  {
    var managerId = UserId.From(3);
    var setup = CreateBoardWithColumns();
    setup.Board.AddMember(managerId, BoardRole.Manager);

    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Board>>(), Arg.Any<CancellationToken>()).Returns(setup.Board);

    var handler = new MoveCardHandler(_repository);
    var command = new MoveCardCommand(setup.Card.Id, setup.Board.Id, setup.TargetColumn.Id, managerId);

    var result = await handler.Handle(command, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.Ok);
    setup.SourceColumn.Cards.ShouldNotContain(setup.Card);
    setup.TargetColumn.Cards.ShouldContain(setup.Card);
    await _repository.Received(1).UpdateAsync(setup.Board, Arg.Any<CancellationToken>());
  }

  private static (Board Board, Column SourceColumn, Column TargetColumn, Card Card) CreateBoardWithColumns()
  {
    var boardId = BoardId.From(50);
    var ownerId = UserId.From(1);
    var board = new Board(BoardName.From("Alpha"), ownerId).WithEntityId(boardId);

    var source = new Column(ColumnName.From("Backlog"), boardId).WithEntityId(ColumnId.From(500));
    var target = new Column(ColumnName.From("Review"), boardId).WithEntityId(ColumnId.From(501));
    board.AddColumn(source);
    board.AddColumn(target);

    var card = new Card(CardTitle.From("Task"), CardDescription.From("Details"), source.Id).WithEntityId(CardId.From(5000));
    source.AddCard(card);

    return (board, source, target, card);
  }
}
