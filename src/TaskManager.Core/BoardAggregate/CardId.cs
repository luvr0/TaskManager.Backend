using Vogen;

namespace TaskManager.Core.BoardAggregate;

[ValueObject<int>]
public readonly partial struct CardId
{
  private static Validation Validate(int value)
    => value > 0 ? Validation.Ok : Validation.Invalid("CardId must be positive.");
}
