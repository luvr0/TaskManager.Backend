namespace TaskManager.Core.BoardAggregate.Events;

public sealed class BoardNameUpdatedEvent(Board board) : DomainEventBase
{
  public Board Board { get; init; } = board;
}
