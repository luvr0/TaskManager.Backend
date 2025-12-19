using TaskManager.Core.BoardAggregate;
using TaskManager.UseCases.Boards;

namespace TaskManager.UseCases.Cards.List;

public record ListCardQuery(BoardId BoardId, ColumnId ColumnId, int? Page = 1, int? PerPage = Constants.DEFAULT_PAGE_SIZE)
  : IQuery<Result<PagedResult<CardDto>>>;
