using TaskManager.Core.UserAggregate.Events;

namespace TaskManager.Core.UserAggregate.Handlers;

public class UserPasswordUpdatedHandler(ILogger<UserPasswordUpdatedHandler> logger) : INotificationHandler<UserPasswordUpdatedEvent>
{
  public async ValueTask Handle(UserPasswordUpdatedEvent domainEvent, CancellationToken cancellationToken)
  {
    logger.LogInformation("Handling User Password Updated event for {userId}", domainEvent.User.Id);
    await ValueTask.CompletedTask;
  }
}
