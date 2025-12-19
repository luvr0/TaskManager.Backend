using TaskManager.Core.UserAggregate;
using TaskManager.Core.UserAggregate.Specifications;
using Microsoft.Extensions.Logging;

namespace TaskManager.UseCases.Users.Get;

public class GetUserHandler(IRepository<User> repository, ILogger<GetUserHandler> logger)
  : IQueryHandler<GetUserQuery, Result<UserDTO>>
{
  public async ValueTask<Result<UserDTO>> Handle(GetUserQuery query, CancellationToken cancellationToken)
  {
    logger.LogInformation("Getting user {UserId}", query.UserId);

    var spec = new UserByIdSpec(query.UserId);
    var user = await repository.FirstOrDefaultAsync(spec, cancellationToken);

    if (user == null)
    {
      logger.LogWarning("User not found: {UserId}", query.UserId);
      return Result<UserDTO>.NotFound();
    }

    var dto = UserDtoMapper.MapToDto(user);
    return Result<UserDTO>.Success(dto);
  }
}
