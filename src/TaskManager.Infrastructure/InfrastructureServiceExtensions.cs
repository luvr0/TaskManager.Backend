using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.Data.Queries;
using TaskManager.Infrastructure.Services;
using TaskManager.UseCases.Boards.List;
using TaskManager.UseCases.Columns.List;
using TaskManager.UseCases.Cards.List;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TaskManager.Core.Interfaces;

namespace TaskManager.Infrastructure;
public static class InfrastructureServiceExtensions
{
  public static IServiceCollection AddInfrastructureServices(
    this IServiceCollection services,
    ConfigurationManager config,
    ILogger logger)
  {
    // Try to get connection strings in order of priority:
    // 1. "cleanarchitecture" - provided by Aspire when using .WithReference(cleanArchDb)
    // 2. "DefaultConnection" - traditional SQL Server connection
    // 3. "SqliteConnection" - fallback to SQLite
    string? connectionString = config.GetConnectionString("cleanarchitecture")
                               ?? config.GetConnectionString("DefaultConnection") 
                               ?? config.GetConnectionString("SqliteConnection");
    Guard.Against.Null(connectionString);

    services.AddScoped<EventDispatchInterceptor>();
    services.AddScoped<IDomainEventDispatcher, MediatorDomainEventDispatcher>();

    services.AddDbContext<AppDbContext>((provider, options) =>
    {
      var eventDispatchInterceptor = provider.GetRequiredService<EventDispatchInterceptor>();
      
      // Use SQL Server if Aspire or DefaultConnection is available, otherwise use SQLite
      if (config.GetConnectionString("cleanarchitecture") != null || 
          config.GetConnectionString("DefaultConnection") != null)
      {
        options.UseSqlServer(connectionString);
      }
      else
      {
        options.UseSqlite(connectionString);
      }
      
      options.AddInterceptors(eventDispatchInterceptor);

      // Prevent startup from throwing when the EF model has pending changes by ignoring the
      // PendingModelChangesWarning. Prefer creating a new migration during development.
      options.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
    });

    services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>))
           .AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>))
           .AddScoped<IListBoardQueryService, ListBoardsQueryService>()
           .AddScoped<IListColumnQueryService, ListColumnsQueryService>()
           .AddScoped<IListCardQueryService, ListCardsQueryService>()
           .AddScoped<IPasswordHasher, PasswordHasher>()
           .AddScoped<ITokenService, TokenService>();

    logger.LogInformation("{Project} services registered", "Infrastructure");

    return services;
  }
}
