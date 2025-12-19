using Ardalis.Result;
using Ardalis.Specification;
using TaskManager.Core.BoardAggregate;
using TaskManager.Core.BoardAggregate.Specifications;
using TaskManager.Core.UserAggregate;
using TaskManager.UnitTests.TestUtils;
using TaskManager.UseCases.Cards.Update;

namespace TaskManager.UnitTests.UseCases.Cards;

public class UpdateCardHandlerTests
{
  private readonly IRepository<Board> _repository = Substitute.For<IRepository<Board>>();

  [Fact]
  public async Task Handle_ReturnsNotFound_WhenUserCannotUpdate()
  {
    var ownerId = UserId.From(1);
    var readerId = UserId.From(2);
    var (board, card) = CreateBoardWithCard();
    board.AddMember(readerId, BoardRole.Reader);

    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Board>>(), Arg.Any<CancellationToken>())
      .Returns(board);

    var handler = new UpdateCardHandler(_repository);
    var command = new UpdateCardCommand(card.Id, board.Id, CardTitle.From("Updated"), null, null, readerId);

    var result = await handler.Handle(command, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.NotFound);
    card.Title.Value.ShouldBe("Task");
    await _repository.DidNotReceive().UpdateAsync(board, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_UpdatesCard_WhenUserHasPermission()
  {
    var ownerId = UserId.From(1);
    var managerId = UserId.From(3);
    var (board, card) = CreateBoardWithCard();
    board.AddMember(managerId, BoardRole.Manager);

    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Board>>(), Arg.Any<CancellationToken>())
      .Returns(board);

    var handler = new UpdateCardHandler(_repository);
    var command = new UpdateCardCommand(card.Id, board.Id, CardTitle.From("Updated"), null, null, managerId);

    var result = await handler.Handle(command, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.Ok);
    card.Title.Value.ShouldBe("Updated");
    await _repository.Received(1).UpdateAsync(board, Arg.Any<CancellationToken>());
  }

  private static (Board board, Card card) CreateBoardWithCard()
  {
    var boardId = BoardId.From(30);
    var board = new Board(BoardName.From("Alpha"), UserId.From(1)).WithEntityId(boardId);
    var column = new Column(ColumnName.From("Backlog"), boardId).WithEntityId(ColumnId.From(300));
    board.AddColumn(column);

    var card = new Card(CardTitle.From("Task"), CardDescription.From("Details"), column.Id).WithEntityId(CardId.From(3000));
    column.AddCard(card);

    return (board, card);
  }
}
