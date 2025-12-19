namespace TaskManager.Core.BoardAggregate.Events;

public sealed class CardTitleUpdatedEvent(Card card) : DomainEventBase
{
  public Card Card { get; init; } = card;
}
