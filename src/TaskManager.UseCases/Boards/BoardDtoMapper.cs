using TaskManager.Core.BoardAggregate;

namespace TaskManager.UseCases.Boards;

public static class BoardDtoMapper
{
  public static BoardDto ToDto(this Board board)
  {
    var members = board.Members.Select(m => new MemberDto(m.UserId, m.Role, string.Empty, string.Empty)).ToList();
    var columns = board.Columns.Select(c => c.ToDto()).ToList();

    return new BoardDto(board.Id, board.Name, board.UserId, members, columns);
  }

  public static ColumnDto ToDto(this Column column)
  {
    var cards = column.Cards.Select(c => c.ToDto()).ToList();
    return new ColumnDto(column.Id, column.Name, column.ColumnOrder, column.BoardId, cards);
  }

  public static CardDto ToDto(this Card card)
  {
    return new CardDto(card.Id, card.Title, card.Description, card.CardOrder, card.Status, card.ColumnId);
  }
}
