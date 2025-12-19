using Vogen;

namespace TaskManager.Core.BoardAggregate;

[ValueObject<string>(conversions: Conversions.SystemTextJson)]
public partial struct CardDescription
{
  public const int MaxLength = 500;
  private static Validation Validate(in string description) =>
    description == null || description.Length == 0
      ? Validation.Ok
      : description.Length > MaxLength
        ? Validation.Invalid($"Card Description cannot be longer than {MaxLength} characters")
        : Validation.Ok;
}
