using TaskManager.Core.BoardAggregate.Events;

namespace TaskManager.Core.BoardAggregate.Handlers;

public class ColumnDeletedHandler : INotificationHandler<ColumnDeletedEvent>
{
  public ValueTask Handle(ColumnDeletedEvent notification, CancellationToken cancellationToken)
  {
    // Log or perform actions when a column is deleted
    return ValueTask.CompletedTask;
  }
}
