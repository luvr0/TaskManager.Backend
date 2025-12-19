namespace TaskManager.Core.BoardAggregate.Events;

public sealed class ColumnNameUpdatedEvent(Column column) : DomainEventBase
{
  public Column Column { get; init; } = column;
}
