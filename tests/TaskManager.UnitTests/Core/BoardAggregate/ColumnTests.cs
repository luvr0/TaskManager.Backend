using System.Reflection;
using TaskManager.Core.BoardAggregate;

namespace TaskManager.UnitTests.Core.BoardAggregate;

public class ColumnTests
{
  [Fact]
  public void AddCard_SetsIncrementingOrderForExistingCards()
  {
    var column = new Column(ColumnName.From("Backlog"), BoardId.From(7));
    SetId(column, ColumnId.From(10));

    var firstCard = new Card(CardTitle.From("Wireframes"), CardDescription.From("Create wireframes"), column.Id);
    var secondCard = new Card(CardTitle.From("Copy"), CardDescription.From("Draft marketing copy"), column.Id);

    column.AddCard(firstCard);
    column.AddCard(secondCard);

    firstCard.CardOrder.Value.ShouldBe(1);
    secondCard.CardOrder.Value.ShouldBe(2);
  }

  [Fact]
  public void AddCard_ConvenienceOverloadAddsCardToColumn()
  {
    var column = new Column(ColumnName.From("Doing"), BoardId.From(9));
    SetId(column, ColumnId.From(11));

    var createdCard = column.AddCard(CardTitle.From("Implement UI"), CardDescription.From("Add Blazor components"));

    column.Cards.ShouldContain(createdCard);
    createdCard.ColumnId.ShouldBe(column.Id);
    createdCard.CardOrder.Value.ShouldBe(1);
  }

  private static void SetId(object entity, object idValue)
  {
    var prop = entity.GetType().GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    if (prop != null && prop.CanWrite)
    {
      prop.SetValue(entity, idValue);
      return;
    }

    var field = entity.GetType().GetField("<Id>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
    if (field != null) field.SetValue(entity, idValue);
  }
}
