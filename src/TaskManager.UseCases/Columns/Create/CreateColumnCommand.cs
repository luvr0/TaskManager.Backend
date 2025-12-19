using TaskManager.Core.BoardAggregate;
using TaskManager.Core.UserAggregate;

namespace TaskManager.UseCases.Columns.Create;

public record CreateColumnCommand(
  ColumnName Name,
  BoardId BoardId,
  UserId RequestingUserId) : ICommand<Result<ColumnId>>;
