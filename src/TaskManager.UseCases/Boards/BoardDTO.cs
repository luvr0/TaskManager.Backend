using TaskManager.Core.BoardAggregate;
using TaskManager.Core.UserAggregate;

namespace TaskManager.UseCases.Boards;

public record MemberDto(UserId Id, BoardRole Role, string Email, string EmailAlias);
public record CardDto(CardId Id, CardTitle Title, CardDescription? Description, CardOrder Order, CardStatus Status, ColumnId ColumnId);
public record ColumnDto(ColumnId Id, ColumnName Name, ColumnOrder Order, BoardId BoardId, IReadOnlyList<CardDto> Cards);
public record BoardDto(BoardId Id, BoardName Name, UserId OwnerId, IReadOnlyList<MemberDto> Members, IReadOnlyList<ColumnDto> Columns);
