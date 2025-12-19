using TaskManager.Core.UserAggregate;
using TaskManager.UseCases.Boards;
using TaskManager.UseCases.Boards.List;

namespace TaskManager.Infrastructure.Data.Queries;

public class ListBoardsQueryService : IListBoardQueryService
{
  private readonly AppDbContext _db;

  public ListBoardsQueryService(AppDbContext db)
  {
    _db = db;
  }

  public async Task<UseCases.PagedResult<BoardDto>> ListAsync(UserId userId, int page, int perPage)
  {
    var filterUserId = userId;
    var query = _db.Boards
      .Where(b => b.UserId == filterUserId || b.Members.Any(m => m.UserId == filterUserId));

    int totalCount = await query.CountAsync();
    int totalPages = (int)Math.Ceiling(totalCount / (double)perPage);

    var rawItems = await query
      .OrderBy(b => b.Id)
      .Skip((page - 1) * perPage)
      .Take(perPage)
      .Select(b => new BoardDto(
        b.Id,
        b.Name,
        b.UserId,
        b.Members.Select(member => new MemberDto(
          member.UserId,
          member.Role,
          _db.Users.Where(u => u.Id == member.UserId).Select(u => u.Email.Value).FirstOrDefault() ?? string.Empty,
          string.Empty)).ToList(),
        Array.Empty<ColumnDto>()))
      .AsNoTracking()
      .ToListAsync();

    var items = rawItems
      .Select(board => board with
      {
        Members = board.Members
          .Select(m => new MemberDto(m.Id, m.Role, m.Email, BuildEmailAlias(m.Email)))
          .ToList()
      })
      .ToList();

    var result = new UseCases.PagedResult<BoardDto>(items, page, perPage, totalCount, totalPages);
    return result;
  }

  private static string BuildEmailAlias(string email)
  {
    if (string.IsNullOrWhiteSpace(email)) return string.Empty;

    var atIndex = email.IndexOf('@');
    if (atIndex <= 0) return string.Empty;

    var local = email[..atIndex];
    if (string.IsNullOrWhiteSpace(local)) return string.Empty;

    var segments = local.Split(['.', '-', '_'], StringSplitOptions.RemoveEmptyEntries);
    if (segments.Length == 0)
    {
      segments = new[] { local };
    }

    var initials = string.Concat(segments.Select(s => char.ToUpperInvariant(s[0])));
    return initials;
  }
}
