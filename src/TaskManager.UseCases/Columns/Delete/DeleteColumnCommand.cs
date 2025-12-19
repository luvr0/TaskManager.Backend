using TaskManager.Core.BoardAggregate;
using TaskManager.Core.UserAggregate;

namespace TaskManager.UseCases.Columns.Delete;

public record DeleteColumnCommand(ColumnId ColumnId, BoardId BoardId, UserId RequestingUserId) : ICommand<Result>;
