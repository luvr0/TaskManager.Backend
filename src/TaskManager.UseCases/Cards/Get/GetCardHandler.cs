using TaskManager.Core.BoardAggregate;
using TaskManager.Core.BoardAggregate.Specifications;
using TaskManager.UseCases.Boards;

namespace TaskManager.UseCases.Cards.Get;

public class GetCardHandler(IReadRepository<Board> repository)
  : IQueryHandler<GetCardQuery, Result<CardDto>>
{
  public async ValueTask<Result<CardDto>> Handle(GetCardQuery request, 
                                                  CancellationToken cancellationToken)
  {
    var spec = new BoardByIdSpec(request.BoardId);
    var board = await repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (board == null) return Result.NotFound();

    Card? card = null;
    foreach (var column in board.Columns)
    {
      card = column.Cards.FirstOrDefault(c => c.Id == request.CardId);
      if (card != null) break;
    }
    
    if (card == null) return Result.NotFound();

    return card.ToDto();
  }
}
