using Vogen;

namespace TaskManager.Core.BoardAggregate;

[ValueObject<string>(conversions: Conversions.SystemTextJson)]
public partial struct CardTitle
{
  public const int MaxLength = 100;
  private static Validation Validate(in string title) =>
    string.IsNullOrEmpty(title)
      ? Validation.Invalid("Card Title cannot be empty")
      : title.Length > MaxLength
        ? Validation.Invalid($"Card Title cannot be longer than {MaxLength} characters")
        : Validation.Ok;
}
