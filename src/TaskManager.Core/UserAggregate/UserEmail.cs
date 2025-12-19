using Vogen;
using System.Text.RegularExpressions;

namespace TaskManager.Core.UserAggregate;

[ValueObject<string>(conversions: Conversions.SystemTextJson)]
public partial struct UserEmail
{
  public const int MaxLength = 200;

  private static readonly Regex EmailRegex =
      new(@"^[a-zA-Z0-9._%+-]+@(gmail\.com|hotmail\.com|outlook\.com|yahoo\.com)$",
          RegexOptions.IgnoreCase | RegexOptions.Compiled);

  private static Validation Validate(in string email) =>
      string.IsNullOrWhiteSpace(email)
          ? Validation.Invalid("Email cannot be empty")
      : email.Length > MaxLength
          ? Validation.Invalid($"Email cannot be longer than {MaxLength} characters")
      : !EmailRegex.IsMatch(email)
          ? Validation.Invalid("Email must belong to a supported provider: gmail.com, hotmail.com, outlook.com, yahoo.com.")
      : Validation.Ok;
}
