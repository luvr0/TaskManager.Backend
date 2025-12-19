using TaskManager.Core.BoardAggregate;
using TaskManager.Core.UserAggregate;

namespace TaskManager.Infrastructure.Data;
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
  public DbSet<Board> Boards => Set<Board>();
  public DbSet<Column> Columns => Set<Column>();
  public DbSet<Card> Cards => Set<Card>();
  public DbSet<User> Users => Set<User>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }

  public override int SaveChanges() =>
        SaveChangesAsync().GetAwaiter().GetResult();
}
