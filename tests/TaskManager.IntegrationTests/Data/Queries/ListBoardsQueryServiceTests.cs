using System;
using System.Linq;
using TaskManager.Core.BoardAggregate;
using TaskManager.Core.UserAggregate;
using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.Data.Queries;

namespace TaskManager.IntegrationTests.Data.Queries;

public class ListBoardsQueryServiceTests
{
  [Fact]
  public async Task ListAsync_ReturnsOwnedBoardsWithMemberAliases()
  {
    await using var dbContext = CreateContext();
    var service = new ListBoardsQueryService(dbContext);

    var owner = CreateUser("owner@gmail.com", "Owner One");
    var collaborator = CreateUser("maria.silva@gmail.com", "Maria Silva");
    dbContext.Users.AddRange(owner, collaborator);
    await dbContext.SaveChangesAsync();

    var board = new Board(BoardName.From("Roadmap"), owner.Id);
    board.AddMember(collaborator.Id, BoardRole.Editor);
    dbContext.Boards.Add(board);
    await dbContext.SaveChangesAsync();

    var result = await service.ListAsync(owner.Id, page: 1, perPage: 10);

    result.TotalCount.ShouldBe(1);
    result.TotalPages.ShouldBe(1);
    result.Items.ShouldHaveSingleItem();
    var dto = result.Items.Single();
    dto.Name.Value.ShouldBe("Roadmap");
    dto.Members.ShouldHaveSingleItem().EmailAlias.ShouldBe("MS");
  }

  [Fact]
  public async Task ListAsync_FiltersBoardsByOwnershipOrMembership()
  {
    await using var dbContext = CreateContext();
    var service = new ListBoardsQueryService(dbContext);

    var owner = CreateUser("owner@gmail.com", "Owner");
    var guest = CreateUser("guest@gmail.com", "Guest User");
    dbContext.Users.AddRange(owner, guest);
    await dbContext.SaveChangesAsync();

    var ownedBoard = new Board(BoardName.From("Operations"), owner.Id);
    ownedBoard.AddMember(guest.Id, BoardRole.Reader);
    dbContext.Boards.Add(ownedBoard);
    await dbContext.SaveChangesAsync();

    var ownerResult = await service.ListAsync(owner.Id, 1, 5);
    ownerResult.Items.ShouldHaveSingleItem();

    var guestResult = await service.ListAsync(guest.Id, 1, 5);
    guestResult.Items.ShouldHaveSingleItem();

    var outsiderResult = await service.ListAsync(UserId.From(999), 1, 5);
    outsiderResult.Items.ShouldBeEmpty();
    outsiderResult.TotalCount.ShouldBe(0);
  }

  private static AppDbContext CreateContext()
  {
    var options = new DbContextOptionsBuilder<AppDbContext>()
      .UseInMemoryDatabase(Guid.NewGuid().ToString())
      .Options;

    return new AppDbContext(options);
  }

  private static User CreateUser(string email, string name)
  {
    return new User(
      UserName.From(name),
      UserEmail.From(email),
      UserPassword.From($"hashed-{Guid.NewGuid():N}"));
  }
}
