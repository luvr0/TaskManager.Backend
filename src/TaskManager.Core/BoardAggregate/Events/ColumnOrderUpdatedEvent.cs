namespace TaskManager.Core.BoardAggregate.Events;

public sealed class ColumnOrderUpdatedEvent(Column column) : DomainEventBase
{
  public Column Column { get; } = column;
}
