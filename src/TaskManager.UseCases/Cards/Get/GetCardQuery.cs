using TaskManager.Core.BoardAggregate;
using TaskManager.UseCases.Boards;

namespace TaskManager.UseCases.Cards.Get;

public record GetCardQuery(CardId CardId, BoardId BoardId) : IQuery<Result<CardDto>>;
