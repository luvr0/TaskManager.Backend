using TaskManager.Core.BoardAggregate;
using TaskManager.UseCases.Cards.Move;
using TaskManager.Web.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TaskManager.Web.Cards;

public class Move(IMediator mediator)
  : Endpoint<MoveCardRequest, Results<Ok, NotFound, ProblemHttpResult>>
{
  public override void Configure()
  {
    Put(MoveCardRequest.Route);
    Policies("RequireAuthenticatedUser");

    Summary(s =>
    {
      s.Summary = "Move a card";
      s.Description = "Moves a card to a different column and/or position.";
      s.ExampleRequest = new MoveCardRequest { CardId = 1, BoardId = 1, TargetColumnId = 2 };

      s.Responses[200] = "Card moved successfully";
      s.Responses[401] = "Authentication required";
      s.Responses[404] = "Card, Board or target column not found";
      s.Responses[400] = "Invalid request or move failed";
    });

    Tags("Cards");

    Description(builder => builder
      .Accepts<MoveCardRequest>("application/json")
      .Produces(200)
      .ProducesProblem(401)
      .ProducesProblem(404)
      .ProducesProblem(400));
  }

  public override async Task<Results<Ok, NotFound, ProblemHttpResult>>
    ExecuteAsync(MoveCardRequest request, CancellationToken ct)
  {
    if (!HttpContext.TryGetUserId(out var userId))
    {
      return TypedResults.Problem(title: "Unauthorized", statusCode: StatusCodes.Status401Unauthorized);
    }

    var cmd = new MoveCardCommand(
      CardId.From(request.CardId),
      BoardId.From(request.BoardId),
      ColumnId.From(request.TargetColumnId),
      userId);

    var result = await mediator.Send(cmd, ct);

    return result.ToMoveResult();
  }
}
