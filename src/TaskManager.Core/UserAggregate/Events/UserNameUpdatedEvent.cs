namespace TaskManager.Core.UserAggregate.Events;

public sealed class UserNameUpdatedEvent(User user) : DomainEventBase
{
  public User User { get; init; } = user;
}
