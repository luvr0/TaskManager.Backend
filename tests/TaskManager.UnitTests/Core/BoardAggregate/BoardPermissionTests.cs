using TaskManager.Core.BoardAggregate;
using TaskManager.Core.UserAggregate;

namespace TaskManager.UnitTests.Core.BoardAggregate;

public class BoardPermissionTests
{
  public static IEnumerable<object[]> PermissionMatrix => new[]
  {
    new object[] { BoardRole.Reader, BoardPermission.Create, false },
    new object[] { BoardRole.Reader, BoardPermission.Read, true },
    new object[] { BoardRole.Reader, BoardPermission.Update, false },
    new object[] { BoardRole.Reader, BoardPermission.Delete, false },
    new object[] { BoardRole.Editor, BoardPermission.Create, false },
    new object[] { BoardRole.Editor, BoardPermission.Read, true },
    new object[] { BoardRole.Editor, BoardPermission.Update, true },
    new object[] { BoardRole.Editor, BoardPermission.Delete, false },
    new object[] { BoardRole.Manager, BoardPermission.Create, true },
    new object[] { BoardRole.Manager, BoardPermission.Read, true },
    new object[] { BoardRole.Manager, BoardPermission.Update, true },
    new object[] { BoardRole.Manager, BoardPermission.Delete, true }
  };

  [Theory]
  [MemberData(nameof(PermissionMatrix))]
  public void BoardRolesRespectPermissionMatrix(BoardRole role, BoardPermission permission, bool expected)
  {
    var member = new BoardMember(UserId.From(999), role);

    member.HasPermission(permission).ShouldBe(expected);
  }

  [Fact]
  public void ReaderPermissions_ShouldOnlyAllowRead()
  {
    var permissions = BoardPermission.GetPermissionsForBoardRole(BoardRole.Reader);

    permissions.ShouldBe(new[] { BoardPermission.Read });
  }

  [Fact]
  public void EditorPermissions_ShouldAllowReadAndUpdate()
  {
    var permissions = BoardPermission.GetPermissionsForBoardRole(BoardRole.Editor);

    permissions.ShouldBe(new[] { BoardPermission.Read, BoardPermission.Update });
  }

  [Fact]
  public void ManagerPermissions_ShouldAllowAllActions()
  {
    var permissions = BoardPermission.GetPermissionsForBoardRole(BoardRole.Manager);

    permissions.ShouldBe(BoardPermission.GetAllBoardPermissions());
  }
}
