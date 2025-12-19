using TaskManager.Core.BoardAggregate.Events;

namespace TaskManager.Core.BoardAggregate.Handlers;

public class CardTitleUpdatedHandler(ILogger<CardTitleUpdatedHandler> logger) : INotificationHandler<CardTitleUpdatedEvent>
{
  public async ValueTask Handle(CardTitleUpdatedEvent domainEvent, CancellationToken cancellationToken)
  {
    logger.LogInformation("Handling Card Title Updated event for {cardId}", domainEvent.Card.Id);
    await ValueTask.CompletedTask;
  }
}
