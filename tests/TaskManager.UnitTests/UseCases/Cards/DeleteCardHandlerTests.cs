using Ardalis.Result;
using Ardalis.Specification;
using TaskManager.Core.BoardAggregate;
using TaskManager.Core.BoardAggregate.Specifications;
using TaskManager.Core.UserAggregate;
using TaskManager.UnitTests.TestUtils;
using TaskManager.UseCases.Cards.Delete;

namespace TaskManager.UnitTests.UseCases.Cards;

public class DeleteCardHandlerTests
{
  private readonly IRepository<Board> _repository = Substitute.For<IRepository<Board>>();

  [Fact]
  public async Task Handle_ReturnsNotFound_WhenUserCannotDelete()
  {
    var readerId = UserId.From(2);
    var (board, card, column) = CreateBoardWithCard();
    board.AddMember(readerId, BoardRole.Reader);

    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Board>>(), Arg.Any<CancellationToken>()).Returns(board);

    var handler = new DeleteCardHandler(_repository);
    var command = new DeleteCardCommand(card.Id, board.Id, readerId);

    var result = await handler.Handle(command, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.NotFound);
    column.Cards.ShouldContain(card);
    await _repository.DidNotReceive().UpdateAsync(board, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_RemovesCard_WhenUserHasDeletePermission()
  {
    var managerId = UserId.From(3);
    var (board, card, column) = CreateBoardWithCard();
    board.AddMember(managerId, BoardRole.Manager);

    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Board>>(), Arg.Any<CancellationToken>()).Returns(board);

    var handler = new DeleteCardHandler(_repository);
    var command = new DeleteCardCommand(card.Id, board.Id, managerId);

    var result = await handler.Handle(command, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.Ok);
    column.Cards.ShouldNotContain(card);
    await _repository.Received(1).UpdateAsync(board, Arg.Any<CancellationToken>());
  }

  private static (Board board, Card card, Column column) CreateBoardWithCard()
  {
    var boardId = BoardId.From(40);
    var ownerId = UserId.From(1);
    var board = new Board(BoardName.From("Alpha"), ownerId).WithEntityId(boardId);
    var column = new Column(ColumnName.From("Backlog"), boardId).WithEntityId(ColumnId.From(400));
    board.AddColumn(column);

    var card = new Card(CardTitle.From("Task"), CardDescription.From("Details"), column.Id).WithEntityId(CardId.From(4000));
    column.AddCard(card);

    return (board, card, column);
  }
}
