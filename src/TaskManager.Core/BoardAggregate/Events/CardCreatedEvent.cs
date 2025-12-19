namespace TaskManager.Core.BoardAggregate.Events;

public sealed class CardCreatedEvent(Card card) : DomainEventBase
{
  public Card Card { get; } = card;
}
