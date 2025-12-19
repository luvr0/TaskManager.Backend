using TaskManager.Core.BoardAggregate.Events;

namespace TaskManager.Core.BoardAggregate.Handlers;

public class CardOrderUpdatedHandler : INotificationHandler<CardOrderUpdatedEvent>
{
  public ValueTask Handle(CardOrderUpdatedEvent notification, CancellationToken cancellationToken)
  {
    // Log or perform actions when a card order is updated
    return ValueTask.CompletedTask;
  }
}
