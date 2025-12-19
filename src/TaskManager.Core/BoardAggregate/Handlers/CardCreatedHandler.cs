using TaskManager.Core.BoardAggregate.Events;

namespace TaskManager.Core.BoardAggregate.Handlers;

public class CardCreatedHandler : INotificationHandler<CardCreatedEvent>
{
  public ValueTask Handle(CardCreatedEvent notification, CancellationToken cancellationToken)
  {
    // Log or perform actions when a card is created
    return ValueTask.CompletedTask;
  }
}
