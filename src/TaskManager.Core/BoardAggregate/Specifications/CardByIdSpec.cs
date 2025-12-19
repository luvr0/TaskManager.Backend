namespace TaskManager.Core.BoardAggregate.Specifications;

public class CardByIdSpec : Specification<Card>
{
  public CardByIdSpec(CardId cardId) =>
    Query
      .Where(card => card.Id == cardId);
}
