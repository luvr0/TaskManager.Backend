using TaskManager.Core.UserAggregate;

namespace TaskManager.UseCases.Users;

public record UserDTO(
  int Id,
  string Name,
  string Email
);

public static class UserDtoMapper
{
  public static UserDTO MapToDto(User user)
  {
    return new UserDTO(
      user.Id.Value,
      user.Name.Value,
      user.Email.Value
    );
  }
}
