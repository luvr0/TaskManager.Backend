namespace TaskManager.Core.BoardAggregate.Events;

public sealed class ColumnDeletedEvent(Column column) : DomainEventBase
{
  public Column Column { get; } = column;
}
