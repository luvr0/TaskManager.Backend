using TaskManager.Core.BoardAggregate;
using TaskManager.UseCases.Boards;

namespace TaskManager.UseCases.Columns.List;

public interface IListColumnQueryService
{
  Task<PagedResult<ColumnDto>> ListAsync(BoardId boardId, int page, int perPage);
}
