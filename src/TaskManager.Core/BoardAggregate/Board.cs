using TaskManager.Core.UserAggregate;
using TaskManager.Core.BoardAggregate.Events;

namespace TaskManager.Core.BoardAggregate;

public class Board : EntityBase<Board, BoardId>, IAggregateRoot
{
  public BoardName Name { get; private set; }
  public UserId UserId { get; private set; }
  private readonly List<BoardMember> _members = new List<BoardMember>();
  private readonly List<Column> _columns = new List<Column>();
  public IReadOnlyCollection<BoardMember> Members => _members.AsReadOnly();
  public IReadOnlyCollection<Column> Columns => _columns.AsReadOnly();

  private Board() { }

  public Board(BoardName name, UserId userId)
  {
    Name = name;
    UserId = userId;
  }

  public Board UpdateName(BoardName newName)
  {
    if (Name.Equals(newName)) return this;
    Name = newName;
    RegisterDomainEvent(new BoardNameUpdatedEvent(this));
    return this;
  }

  public Board AddColumn(Column column)
  {
    int nextOrder = _columns.Any() ? _columns.Max(c => c.ColumnOrder.Value) + 1 : 1;
    column.UpdateOrder(ColumnOrder.From(nextOrder));

    _columns.Add(column);
    return this;
  }

  public Board RemoveColumn(Column column)
  {
    _columns.Remove(column);
    return this;
  }

  public Board AddMember(UserId userId, BoardRole role)
  {
    if (_members.Any(m => m.UserId == userId)) return this;
    _members.Add(new BoardMember(userId, role));
    return this;
  }

  public Board RemoveMember(UserId userId)
  {
    var member = _members.FirstOrDefault(m => m.UserId == userId);
    if (member is not null)
    {
      _members.Remove(member);
    }
    return this;
  }

  public Board UpdateMemberRole(UserId userId, BoardRole newRole)
  {
    var memberIndex = _members.FindIndex(m => m.UserId == userId);
    if (memberIndex < 0) return this;

    var existing = _members[memberIndex];
    if (existing.Role == newRole) return this;

    _members[memberIndex] = existing with { Role = newRole };
    return this;
  }

  public BoardMember? GetMember(UserId userId) => _members.FirstOrDefault(m => m.UserId == userId);

  public bool HasPermission(UserId userId, BoardPermission permission)
  {
    if (UserId == userId) return true;
    var member = GetMember(userId);
    return member is not null && member.HasPermission(permission);
  }

  public bool CanBeAccessedBy(UserId userId) => UserId == userId || _members.Any(m => m.UserId == userId);
}

