using TaskManager.Core.UserAggregate;

namespace TaskManager.UseCases.Users.Get;

public record GetUserQuery(UserId UserId) : IQuery<Result<UserDTO>>;
