using TaskManager.Core.BoardAggregate.Events;

namespace TaskManager.Core.BoardAggregate;

public class Card : EntityBase<Card, CardId>
{
  private Card() { }

  public Card(CardTitle title, CardDescription? description, ColumnId columnId)
  {
    Title = title;
    Description = description;
    ColumnId = columnId;
  }

  public CardTitle Title { get; private set; }
  public CardDescription? Description { get; private set; }
  public CardStatus Status { get; private set; } = CardStatus.Pending;
  public ColumnId ColumnId { get; private set; }
  public CardOrder CardOrder { get; private set; }
  
  public Card UpdateTitle(CardTitle newTitle)
  {
    if (Title.Equals(newTitle)) return this;
    Title = newTitle;
    RegisterDomainEvent(new CardTitleUpdatedEvent(this));
    return this;
  }

  public Card UpdateDescription(CardDescription? newDescription)
  {
    if (Equals(Description, newDescription)) return this;
    Description = newDescription;
    RegisterDomainEvent(new CardDescriptionUpdatedEvent(this));
    return this;
  }
  
  public Card UpdateStatus(CardStatus newStatus)
  {
    if (Status.Equals(newStatus)) return this;
    Status = newStatus;
    RegisterDomainEvent(new CardStatusUpdatedEvent(this));
    return this;
  }

  internal Card UpdateOrder(CardOrder order)
  {
    CardOrder = order;
    return this;
  }
}
