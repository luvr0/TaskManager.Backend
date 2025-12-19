using TaskManager.Core.UserAggregate.Events;

namespace TaskManager.Core.UserAggregate;

public class User : EntityBase<User, UserId>, IAggregateRoot
{
  public UserName Name { get; private set; }
  public UserEmail Email { get; private set; }
  public UserPassword PasswordHash { get; private set; }
  public RefreshToken? RefreshToken { get; private set; }
  public DateTime? RefreshTokenExpiryTime { get; private set; }

  private User() { }

  public User(UserName name, UserEmail email, UserPassword passwordHash)
  {
    Name = name;
    Email = email;
    PasswordHash = passwordHash;
    RegisterDomainEvent(new UserRegisteredEvent(Id));
  }

  public User UpdateName(UserName newName)
  {
    if (Name == newName) return this;
    Name = newName;
    RegisterDomainEvent(new UserNameUpdatedEvent(this));
    return this;
  }

  public User UpdateEmail(UserEmail newEmail)
  {
    if (Email == newEmail) return this;
    Email = newEmail;
    RegisterDomainEvent(new UserEmailUpdatedEvent(this));
    return this;
  }

  public User UpdatePassword(UserPassword newPasswordHash)
  {
    if (PasswordHash == newPasswordHash) return this;
    PasswordHash = newPasswordHash;
    RegisterDomainEvent(new UserPasswordUpdatedEvent(this));
    return this;
  }

  public User SetRefreshToken(RefreshToken refreshToken, DateTime expiryTime)
  {
    RefreshToken = refreshToken;
    RefreshTokenExpiryTime = expiryTime;
    return this;
  }

  public User ClearRefreshToken()
  {
    RefreshToken = null;
    RefreshTokenExpiryTime = null;
    return this;
  }

  public bool IsRefreshTokenValid() => RefreshToken != null &&
                                       RefreshTokenExpiryTime.HasValue &&
                                       RefreshTokenExpiryTime.Value > DateTime.UtcNow;
}
