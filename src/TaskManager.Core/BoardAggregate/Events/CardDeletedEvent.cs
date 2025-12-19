namespace TaskManager.Core.BoardAggregate.Events;

public sealed class CardDeletedEvent(Card card) : DomainEventBase
{
  public Card Card { get; } = card;
}
