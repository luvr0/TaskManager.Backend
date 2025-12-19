using TaskManager.Core.UserAggregate;

namespace TaskManager.UseCases.Boards.List;

public record ListBoardQuery(UserId UserId, int? Page = 1, int? PerPage = Constants.DEFAULT_PAGE_SIZE)
  : IQuery<Result<PagedResult<BoardDto>>>;
