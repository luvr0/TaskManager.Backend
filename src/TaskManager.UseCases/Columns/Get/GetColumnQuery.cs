using TaskManager.Core.BoardAggregate;
using TaskManager.Core.BoardAggregate.Specifications;
using TaskManager.UseCases.Boards;

namespace TaskManager.UseCases.Columns.Get;

public record GetColumnQuery(ColumnId ColumnId, BoardId BoardId) : IQuery<Result<ColumnDto>>;
