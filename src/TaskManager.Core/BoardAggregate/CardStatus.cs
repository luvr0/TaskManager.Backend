namespace TaskManager.Core.BoardAggregate;

public class CardStatus : SmartEnum<CardStatus>
{
  public static readonly CardStatus Pending = new(nameof(Pending), 0);
  public static readonly CardStatus Done = new(nameof(Done), 1);
  public static readonly CardStatus Incomplete = new(nameof(Incomplete), 2);

  protected CardStatus(string name, int value) : base(name, value) { }
}
