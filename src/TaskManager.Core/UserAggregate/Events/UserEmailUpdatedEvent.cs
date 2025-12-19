namespace TaskManager.Core.UserAggregate.Events;

public sealed class UserEmailUpdatedEvent(User user) : DomainEventBase
{
  public User User { get; init; } = user;
}
