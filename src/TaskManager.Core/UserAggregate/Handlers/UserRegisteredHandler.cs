using TaskManager.Core.UserAggregate.Events;

namespace TaskManager.Core.UserAggregate.Handlers;

public class UserRegisteredHandler(ILogger<UserRegisteredHandler> logger) : INotificationHandler<UserRegisteredEvent>
{
  public async ValueTask Handle(UserRegisteredEvent domainEvent, CancellationToken cancellationToken)
  {
    logger.LogInformation("Handling User Registered event for {userId}", domainEvent.UserId);
    await ValueTask.CompletedTask;
  }
}
