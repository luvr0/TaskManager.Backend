using TaskManager.Core.UserAggregate.Events;

namespace TaskManager.Core.UserAggregate.Handlers;

public class UserNameUpdatedHandler(ILogger<UserNameUpdatedHandler> logger) : INotificationHandler<UserNameUpdatedEvent>
{
  public async ValueTask Handle(UserNameUpdatedEvent domainEvent, CancellationToken cancellationToken)
  {
    logger.LogInformation("Handling User Name Updated event for {userId}", domainEvent.User.Id);
    await ValueTask.CompletedTask;
  }
}
