using Vogen;

namespace TaskManager.Core.BoardAggregate;

[ValueObject<int>]
public readonly partial struct CardOrder
{
  private static Validation Validate(int value)
    => value > 0 ? Validation.Ok : Validation.Invalid("Card Order must be positive.");
}
