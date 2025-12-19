using TaskManager.Core.BoardAggregate;
using TaskManager.Core.UserAggregate;
using System.Linq;

namespace TaskManager.UnitTests.Core.BoardAggregate;

public class BoardTests
{
  [Fact]
  public void AddMember_AddsMember_WhenUserIsNotPresent()
  {
    var board = CreateBoard();
    var memberId = UserId.From(2);

    board.AddMember(memberId, BoardRole.Editor);

    board.Members.ShouldContain(m => m.UserId == memberId && m.Role == BoardRole.Editor);
  }

  [Fact]
  public void AddMember_DoesNotAddDuplicateMembers()
  {
    var board = CreateBoard();
    var memberId = UserId.From(2);

    board.AddMember(memberId, BoardRole.Reader);
    board.AddMember(memberId, BoardRole.Manager);

    board.Members.Count(m => m.UserId == memberId).ShouldBe(1);
    board.Members.First(m => m.UserId == memberId).Role.ShouldBe(BoardRole.Reader);
  }

  [Fact]
  public void AddColumn_AssignsSequentialOrder()
  {
    var board = CreateBoard();
    var firstColumn = new Column(ColumnName.From("Backlog"), BoardId.From(1));
    var secondColumn = new Column(ColumnName.From("In Progress"), BoardId.From(1));

    board.AddColumn(firstColumn);
    board.AddColumn(secondColumn);

    firstColumn.ColumnOrder.Value.ShouldBe(1);
    secondColumn.ColumnOrder.Value.ShouldBe(2);
  }

  [Fact]
  public void CanBeAccessedBy_ReturnsExpectedValues()
  {
    var ownerId = UserId.From(1);
    var board = new Board(BoardName.From("Roadmap"), ownerId);
    var collaboratorId = UserId.From(42);
    var outsiderId = UserId.From(100);

    board.AddMember(collaboratorId, BoardRole.Editor);

    board.CanBeAccessedBy(ownerId).ShouldBeTrue();
    board.CanBeAccessedBy(collaboratorId).ShouldBeTrue();
    board.CanBeAccessedBy(outsiderId).ShouldBeFalse();
  }

  [Fact]
  public void UpdateMemberRole_UpdatesRole_WhenMemberExists()
  {
    var board = CreateBoard();
    var memberId = UserId.From(2);
    board.AddMember(memberId, BoardRole.Reader);

    board.UpdateMemberRole(memberId, BoardRole.Manager);

    board.Members.First(m => m.UserId == memberId).Role.ShouldBe(BoardRole.Manager);
  }

  [Fact]
  public void UpdateMemberRole_DoesNothing_WhenMemberDoesNotExist()
  {
    var board = CreateBoard();
    board.AddMember(UserId.From(2), BoardRole.Reader);

    board.UpdateMemberRole(UserId.From(3), BoardRole.Editor);

    board.Members.ShouldAllBe(m => m.Role == BoardRole.Reader);
  }

  [Fact]
  public void HasPermission_ReturnsTrueForOwner()
  {
    var ownerId = UserId.From(1);
    var board = new Board(BoardName.From("Roadmap"), ownerId);

    board.HasPermission(ownerId, BoardPermission.Delete).ShouldBeTrue();
  }

  [Fact]
  public void HasPermission_RespectsMemberRoles()
  {
    var board = CreateBoard();
    var readerId = UserId.From(2);
    var managerId = UserId.From(3);
    board.AddMember(readerId, BoardRole.Reader);
    board.AddMember(managerId, BoardRole.Manager);

    board.HasPermission(readerId, BoardPermission.Update).ShouldBeFalse();
    board.HasPermission(managerId, BoardPermission.Delete).ShouldBeTrue();
  }

  private static Board CreateBoard() => new(BoardName.From("Roadmap"), UserId.From(1));
}
