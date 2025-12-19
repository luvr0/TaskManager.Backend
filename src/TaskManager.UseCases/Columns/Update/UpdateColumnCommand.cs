using TaskManager.Core.BoardAggregate;
using TaskManager.Core.UserAggregate;
using TaskManager.UseCases.Boards;

namespace TaskManager.UseCases.Columns.Update;

public record UpdateColumnCommand(ColumnId ColumnId, BoardId BoardId, ColumnName NewName, UserId RequestingUserId) : ICommand<Result<ColumnDto>>;
