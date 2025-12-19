namespace TaskManager.Core.UserAggregate.Events;

public sealed class UserPasswordUpdatedEvent(User user) : DomainEventBase
{
  public User User { get; init; } = user;
}
