using TaskManager.Core.BoardAggregate;
using TaskManager.UseCases.Boards;

namespace TaskManager.UseCases.Columns.List;

public record ListColumnQuery(BoardId BoardId, int? Page = 1, int? PerPage = Constants.DEFAULT_PAGE_SIZE)
  : IQuery<Result<PagedResult<ColumnDto>>>;
