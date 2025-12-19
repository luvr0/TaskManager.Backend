using TaskManager.Core.BoardAggregate;
using TaskManager.UseCases;
using TaskManager.UseCases.Boards;
using TaskManager.UseCases.Cards.List;
using Microsoft.EntityFrameworkCore;

namespace TaskManager.Infrastructure.Data.Queries;

public class ListCardsQueryService : IListCardQueryService
{
  private readonly AppDbContext _db;

  public ListCardsQueryService(AppDbContext db)
  {
    _db = db;
  }

  public async Task<PagedResult<CardDto>> ListAsync(BoardId boardId, ColumnId columnId, int page, int perPage)
  {
    bool columnExists = await _db.Columns
      .AnyAsync(c => c.Id == columnId && c.BoardId == boardId);

    if (!columnExists)
    {
      return new PagedResult<CardDto>(Array.Empty<CardDto>(), page, perPage, 0, 0);
    }

    var query = _db.Cards
      .AsQueryable()
      .Where(card => card.ColumnId == columnId);

    int totalCount = await query.CountAsync();
    int totalPages = (int)Math.Ceiling(totalCount / (double)perPage);

    var items = await query
      .OrderBy(card => card.CardOrder)
      .ThenBy(card => card.Id)
      .Skip((page - 1) * perPage)
      .Take(perPage)
      .Select(card => new CardDto(
        card.Id,
        card.Title,
        card.Description,
        card.CardOrder,
        card.Status,
        card.ColumnId))
      .AsNoTracking()
      .ToListAsync();

    return new PagedResult<CardDto>(items, page, perPage, totalCount, totalPages);
  }
}
