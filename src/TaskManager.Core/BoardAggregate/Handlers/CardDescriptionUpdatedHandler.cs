using TaskManager.Core.BoardAggregate.Events;

namespace TaskManager.Core.BoardAggregate.Handlers;

public class CardDescriptionUpdatedHandler(ILogger<CardDescriptionUpdatedHandler> logger) : INotificationHandler<CardDescriptionUpdatedEvent>
{
  public async ValueTask Handle(CardDescriptionUpdatedEvent domainEvent, CancellationToken cancellationToken)
  {
    logger.LogInformation("Handling Card Description Updated event for {cardId}", domainEvent.Card.Id);
    await ValueTask.CompletedTask;
  }
}
