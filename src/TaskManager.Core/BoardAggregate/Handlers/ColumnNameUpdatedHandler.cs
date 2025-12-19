using TaskManager.Core.BoardAggregate.Events;

namespace TaskManager.Core.BoardAggregate.Handlers;

public class ColumnNameUpdatedHandler(ILogger<ColumnNameUpdatedHandler> logger) : INotificationHandler<ColumnNameUpdatedEvent>
{
  public async ValueTask Handle(ColumnNameUpdatedEvent domainEvent, CancellationToken cancellationToken)
  {
    logger.LogInformation("Handling Column Name Updated event for {columnId}", domainEvent.Column.Id);
    await ValueTask.CompletedTask;
  }
}
