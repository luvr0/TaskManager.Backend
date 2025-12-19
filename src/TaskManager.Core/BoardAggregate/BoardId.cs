using Vogen;

namespace TaskManager.Core.BoardAggregate;

[ValueObject<int>]
public readonly partial struct BoardId
{
  private static Validation Validate(int value)
    => value > 0 ? Validation.Ok : Validation.Invalid("BoardId must be positive.");
}
