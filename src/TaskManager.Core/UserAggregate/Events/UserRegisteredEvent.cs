namespace TaskManager.Core.UserAggregate.Events;


public sealed class UserRegisteredEvent(UserId userId) : DomainEventBase
{
  public UserId UserId { get; init; } = userId;
}
