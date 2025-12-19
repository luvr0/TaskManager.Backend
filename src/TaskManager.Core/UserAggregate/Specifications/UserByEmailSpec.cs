using TaskManager.Core.UserAggregate;

namespace TaskManager.Core.UserAggregate.Specifications;

public class UserByEmailSpec : Specification<User>
{
  public UserByEmailSpec(UserEmail email)
  {
    Query.Where(user => user.Email == email);
  }
}
