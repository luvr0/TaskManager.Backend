using TaskManager.Core.UserAggregate;

namespace TaskManager.UseCases.Boards.List;

/// <summary>
/// Represents a service that will actually fetch the necessary data
/// Typically implemented in Infrastructure
/// </summary>
public interface IListBoardQueryService
{
  Task<UseCases.PagedResult<BoardDto>> ListAsync(UserId userId, int page, int perPage);
}
