using TaskManager.Core.BoardAggregate.Events;

namespace TaskManager.Core.BoardAggregate.Handlers;

public class CardStatusUpdatedHandler(ILogger<CardStatusUpdatedHandler> logger) : INotificationHandler<CardStatusUpdatedEvent>
{
  public async ValueTask Handle(CardStatusUpdatedEvent domainEvent, CancellationToken cancellationToken)
  {
    logger.LogInformation("Handling Card Status Updated event for {cardId}", domainEvent.Card.Id);
    await ValueTask.CompletedTask;
  }
}
