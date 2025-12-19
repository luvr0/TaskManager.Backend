using TaskManager.Core.BoardAggregate;

namespace TaskManager.UseCases.Boards.Create;

public class CreateBoardHandler(IRepository<Board> repository) : ICommandHandler<CreateBoardCommand, Result<BoardId>>
{
  public async ValueTask<Result<BoardId>> Handle(CreateBoardCommand command, CancellationToken cancellationToken)
  {
    var board = new Board(command.Name, command.OwnerId);

    var created = await repository.AddAsync(board, cancellationToken);

    return created.Id;
  }
}
