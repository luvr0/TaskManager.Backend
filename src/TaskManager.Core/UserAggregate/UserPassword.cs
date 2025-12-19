using Vogen;
using System.Text.RegularExpressions;

namespace TaskManager.Core.UserAggregate;

[ValueObject<string>(conversions: Conversions.SystemTextJson)]
public partial struct UserPassword
{
  public const int MaxLength = 100;

  private static readonly Regex StrongPasswordRegex =
      new(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&.,;:+\-=_(){}[\]^~]).{8,}$");

  private static Validation Validate(in string password) =>
      string.IsNullOrWhiteSpace(password)
          ? Validation.Invalid("Password cannot be empty")
      : password.Length > MaxLength
          ? Validation.Invalid($"Password cannot be longer than {MaxLength} characters")
      : Validation.Ok;

  public static bool IsStrongPassword(string? password) =>
    !string.IsNullOrWhiteSpace(password) && StrongPasswordRegex.IsMatch(password);
}
