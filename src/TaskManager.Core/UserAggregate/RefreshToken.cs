using Vogen;

namespace TaskManager.Core.UserAggregate;

[ValueObject<string>(conversions: Conversions.SystemTextJson)]
public partial struct RefreshToken
{
  public const int MaxLength = 500;

  private static Validation Validate(in string token) =>
    string.IsNullOrWhiteSpace(token)
      ? Validation.Invalid("Refresh token cannot be empty")
      : token.Length > MaxLength
        ? Validation.Invalid($"Refresh token cannot be longer than {MaxLength} characters")
        : Validation.Ok;
}
