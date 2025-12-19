using System.Collections.Generic;
using TaskManager.UseCases.Authentication;

namespace TaskManager.Web.Auth;

public sealed record AuthResponse(
  int UserId,
  string Email,
  string AccessToken,
  DateTime RefreshTokenExpiresAt)
{
  public static AuthResponse FromDto(AuthTokensDto dto) => new(
    dto.UserId,
    dto.Email,
    dto.AccessToken,
    dto.RefreshTokenExpiresAt);
}
