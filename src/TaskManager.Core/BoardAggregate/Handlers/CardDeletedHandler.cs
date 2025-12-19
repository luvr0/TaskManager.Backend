using TaskManager.Core.BoardAggregate.Events;

namespace TaskManager.Core.BoardAggregate.Handlers;

public class CardDeletedHandler : INotificationHandler<CardDeletedEvent>
{
  public ValueTask Handle(CardDeletedEvent notification, CancellationToken cancellationToken)
  {
    // Log or perform actions when a card is deleted
    return ValueTask.CompletedTask;
  }
}
