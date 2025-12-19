using TaskManager.Core.BoardAggregate.Events;

namespace TaskManager.Core.BoardAggregate.Handlers;

public class BoardNameUpdatedHandler(ILogger<BoardNameUpdatedHandler> logger) : INotificationHandler<BoardNameUpdatedEvent>
{
  public async ValueTask Handle(BoardNameUpdatedEvent domainEvent, CancellationToken cancellationToken)
  {
    logger.LogInformation("Handling Board Name Updated event for {boardId}", domainEvent.Board.Id);
    await ValueTask.CompletedTask;
  }
}
