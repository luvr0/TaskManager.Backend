namespace TaskManager.Core.BoardAggregate.Events;

public sealed class CardDescriptionUpdatedEvent(Card card) : DomainEventBase
{
  public Card Card { get; init; } = card;
}
