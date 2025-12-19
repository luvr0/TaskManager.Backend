namespace TaskManager.Core.BoardAggregate.Events;

public sealed class ColumnCreatedEvent(Column column) : DomainEventBase
{
  public Column Column { get; } = column;
}
