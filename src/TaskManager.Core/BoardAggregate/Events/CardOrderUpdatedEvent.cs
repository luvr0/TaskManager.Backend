namespace TaskManager.Core.BoardAggregate.Events;

public sealed class CardOrderUpdatedEvent(Card card) : DomainEventBase
{
  public Card Card { get; } = card;
}
