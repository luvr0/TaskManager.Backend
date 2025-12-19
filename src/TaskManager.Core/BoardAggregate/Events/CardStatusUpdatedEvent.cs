namespace TaskManager.Core.BoardAggregate.Events;

public sealed class CardStatusUpdatedEvent(Card card) : DomainEventBase
{
  public Card Card { get; init; } = card;
}
