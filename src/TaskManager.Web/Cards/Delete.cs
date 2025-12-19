using TaskManager.Core.BoardAggregate;
using TaskManager.UseCases.Cards.Delete;
using TaskManager.Web.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TaskManager.Web.Cards;

public class Delete(IMediator mediator)
  : Endpoint<DeleteCardRequest,
             Results<NoContent,
                     NotFound,
                     ProblemHttpResult>>
{
  public override void Configure()
  {
    Delete(DeleteCardRequest.Route);
    Policies("RequireAuthenticatedUser");

    Summary(s =>
    {
      s.Summary = "Delete a card";
      s.Description = "Deletes an existing card by ID. This action cannot be undone.";
      s.ExampleRequest = new DeleteCardRequest { CardId = 1, BoardId = 1 };

      s.Responses[204] = "Card deleted successfully";
      s.Responses[401] = "Authentication required";
      s.Responses[404] = "Card or Board not found";
      s.Responses[400] = "Invalid request or deletion failed";
    });

    Tags("Cards");

    Description(builder => builder
      .Accepts<DeleteCardRequest>()
      .Produces(204)
      .ProducesProblem(401)
      .ProducesProblem(404)
      .ProducesProblem(400));
  }

  public override async Task<Results<NoContent, NotFound, ProblemHttpResult>>
    ExecuteAsync(DeleteCardRequest req, CancellationToken ct)
  {
    if (!HttpContext.TryGetUserId(out var userId))
    {
      return TypedResults.Problem(title: "Unauthorized", statusCode: StatusCodes.Status401Unauthorized);
    }

    var cmd = new DeleteCardCommand(
      CardId.From(req.CardId), 
      BoardId.From(req.BoardId),
      userId);
    var result = await mediator.Send(cmd, ct);

    return result.ToDeleteResult();
  }
}
