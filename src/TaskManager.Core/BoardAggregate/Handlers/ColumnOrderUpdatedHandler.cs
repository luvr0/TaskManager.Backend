using TaskManager.Core.BoardAggregate.Events;

namespace TaskManager.Core.BoardAggregate.Handlers;

public class ColumnOrderUpdatedHandler : INotificationHandler<ColumnOrderUpdatedEvent>
{
  public ValueTask Handle(ColumnOrderUpdatedEvent notification, CancellationToken cancellationToken)
  {
    // Log or perform actions when a column order is updated
    return ValueTask.CompletedTask;
  }
}
