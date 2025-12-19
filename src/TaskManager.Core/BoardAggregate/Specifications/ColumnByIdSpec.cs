namespace TaskManager.Core.BoardAggregate.Specifications;

public class ColumnByIdSpec : Specification<Column>
{
  public ColumnByIdSpec(ColumnId columnId) =>
    Query
      .Where(column => column.Id == columnId);
}
