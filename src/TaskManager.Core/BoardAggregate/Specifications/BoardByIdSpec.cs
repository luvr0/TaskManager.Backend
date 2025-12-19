namespace TaskManager.Core.BoardAggregate.Specifications;

public class BoardByIdSpec : Specification<Board>
{
  public BoardByIdSpec(BoardId boardId)
  {
    Query
      .Where(board => board.Id == boardId)
      .Include(board => board.Columns)
        .ThenInclude(column => column.Cards)
      .Include(board => board.Members);
  }
}
