namespace TaskManager.Core.UserAggregate.Specifications;

public class UserByIdSpec : Specification<User>
{
  public UserByIdSpec(UserId userId) =>
    Query
        .Where(user => user.Id == userId);
}
