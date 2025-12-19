using Vogen;

namespace TaskManager.Core.BoardAggregate;

[ValueObject<string>(conversions: Conversions.SystemTextJson)]
public partial struct BoardName
{
  public const int MaxLength = 100;
  private static Validation Validate(in string name) =>
    string.IsNullOrEmpty(name)
      ? Validation.Invalid("Board Name cannot be empty")
      : name.Length > MaxLength
        ? Validation.Invalid($"Board Name cannot be longer than {MaxLength} characters")
        : Validation.Ok;
}
