using System.Threading.Tasks;
using TaskManager.Core.BoardAggregate;
using TaskManager.Core.UserAggregate;
using TaskManager.Core.Interfaces;

namespace TaskManager.Infrastructure.Data;

public static class SeedData
{
  public static Task InitializeAsync(AppDbContext dbContext) =>
    Task.CompletedTask;

  public static Task PopulateTestDataAsync(AppDbContext dbContext) =>
    Task.CompletedTask;
}
