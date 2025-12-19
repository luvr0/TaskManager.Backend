using TaskManager.Core.BoardAggregate.Events;

namespace TaskManager.Core.BoardAggregate;

public class Column : EntityBase<Column, ColumnId>
{
  private Column() 
  { 
    ColumnOrder = ColumnOrder.From(1);
  }

  public Column(ColumnName name, BoardId boardId)
  {
    Name = name;
    BoardId = boardId;
    ColumnOrder = ColumnOrder.From(1);
  }

  public ColumnName Name { get; private set; }
  public ColumnOrder ColumnOrder { get; private set; }
  public BoardId BoardId { get; private set; }
  private readonly List<Card> _cards = new List<Card>();
  public IReadOnlyCollection<Card> Cards => _cards.AsReadOnly();

  public Column AddCard(Card card)
  {
    SetCardOrderAndColumn(card);
    _cards.Add(card);
    return this;
  }

  // convenience overload to create card within this column
  public Card AddCard(CardTitle title, CardDescription description)
  {
    var card = new Card(title, description, this.Id);
    SetCardOrderAndColumn(card);
    _cards.Add(card);
    return card;
  }

  private void SetCardOrderAndColumn(Card card)
  {
    int nextOrder = _cards.Any() ? _cards.Max(c => c.CardOrder.Value) + 1 : 1;
    card.UpdateOrder(CardOrder.From(nextOrder));
  }

  public Column RemoveCard(Card card)
  {
    _cards.Remove(card);
    return this;
  }
  
  public Column UpdateName(ColumnName newName)
  {
    if (Name.Equals(newName)) return this;
    Name = newName;
    RegisterDomainEvent(new ColumnNameUpdatedEvent(this));
    return this;
  }

  public Column UpdateOrder(ColumnOrder order)
  {
    ColumnOrder = order;
    return this;
  }
}
