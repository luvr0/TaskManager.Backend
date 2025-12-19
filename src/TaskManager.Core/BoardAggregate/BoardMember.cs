using TaskManager.Core.UserAggregate;

namespace TaskManager.Core.BoardAggregate;

public sealed record BoardMember(UserId UserId, BoardRole Role)
{
  public IReadOnlyCollection<BoardPermission> Permissions => BoardPermission.GetPermissionsForBoardRole(Role);
  
  public bool HasPermission(BoardPermission permission) => Permissions.Contains(permission);
}
