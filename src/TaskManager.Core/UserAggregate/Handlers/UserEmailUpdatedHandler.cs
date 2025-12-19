using TaskManager.Core.UserAggregate.Events;

namespace TaskManager.Core.UserAggregate.Handlers;

public class UserEmailUpdatedHandler(ILogger<UserEmailUpdatedHandler> logger) : INotificationHandler<UserEmailUpdatedEvent>
{
  public async ValueTask Handle(UserEmailUpdatedEvent domainEvent, CancellationToken cancellationToken)
  {
    logger.LogInformation("Handling User Email Updated event for {userId}", domainEvent.User.Id);
    await ValueTask.CompletedTask;
  }
}
