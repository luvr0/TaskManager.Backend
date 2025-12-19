using Vogen;

[assembly: VogenDefaults(
        staticAbstractsGeneration: StaticAbstractsGeneration.MostCommon | StaticAbstractsGeneration.InstanceMethodsAndProperties)]


namespace TaskManager.Core.UserAggregate;

[ValueObject<int>]
public readonly partial struct UserId
{
  private static Validation Validate(int value)
      => value > 0 ? Validation.Ok : Validation.Invalid("UserId must be positive.");
}
