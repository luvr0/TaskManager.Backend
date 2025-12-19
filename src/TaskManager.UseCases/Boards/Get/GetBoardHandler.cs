using TaskManager.Core.BoardAggregate;
using TaskManager.Core.BoardAggregate.Specifications;

namespace TaskManager.UseCases.Boards.Get;

public class GetBoardHandler(IReadRepository<Board> repository)
  : IQueryHandler<GetBoardQuery, Result<BoardDto>>
{
  public async ValueTask<Result<BoardDto>> Handle(GetBoardQuery request, 
                                                  CancellationToken cancellationToken)
  {
    var spec = new BoardByIdSpec(request.BoardId);
    var board = await repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (board == null) return Result.NotFound();

    if (!board.CanBeAccessedBy(request.RequestingUserId))
    {
      return Result.NotFound();
    }

    return board.ToDto();
  }
}
