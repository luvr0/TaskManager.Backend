using TaskManager.Core.BoardAggregate.Events;

namespace TaskManager.Core.BoardAggregate.Handlers;

public class ColumnCreatedHandler : INotificationHandler<ColumnCreatedEvent>
{
  public ValueTask Handle(ColumnCreatedEvent notification, CancellationToken cancellationToken)
  {
    // Log or perform actions when a column is created
    return ValueTask.CompletedTask;
  }
}
