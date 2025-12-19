using TaskManager.Core.BoardAggregate;
using TaskManager.UseCases;
using TaskManager.UseCases.Boards;
using TaskManager.UseCases.Columns.List;
using Microsoft.EntityFrameworkCore;

namespace TaskManager.Infrastructure.Data.Queries;

public class ListColumnsQueryService : IListColumnQueryService
{
  private readonly AppDbContext _db;

  public ListColumnsQueryService(AppDbContext db)
  {
    _db = db;
  }

  public async Task<PagedResult<ColumnDto>> ListAsync(BoardId boardId, int page, int perPage)
  {
    var filterBoardId = boardId;
    var query = _db.Columns
      .AsQueryable()
      .Where(c => c.BoardId == filterBoardId);

    int totalCount = await query.CountAsync();
    int totalPages = (int)Math.Ceiling(totalCount / (double)perPage);

    var items = await query
      .OrderBy(c => c.ColumnOrder)
      .ThenBy(c => c.Id)
      .Skip((page - 1) * perPage)
      .Take(perPage)
      .Select(c => new ColumnDto(
        c.Id,
        c.Name,
        c.ColumnOrder,
        c.BoardId,
        Array.Empty<CardDto>()))
      .AsNoTracking()
      .ToListAsync();

    return new PagedResult<ColumnDto>(items, page, perPage, totalCount, totalPages);
  }
}
