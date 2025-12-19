using TaskManager.Core.BoardAggregate;
using TaskManager.UseCases.Boards;

namespace TaskManager.UseCases.Cards.List;

public interface IListCardQueryService
{
  Task<PagedResult<CardDto>> ListAsync(BoardId boardId, ColumnId columnId, int page, int perPage);
}
