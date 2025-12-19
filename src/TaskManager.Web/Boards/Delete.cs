using TaskManager.Core.BoardAggregate;
using TaskManager.UseCases.Boards.Delete;
using TaskManager.Web.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TaskManager.Web.Boards;

public class Delete(IMediator mediator)
  : Endpoint<DeleteBoardRequest,
             Results<NoContent, NotFound, ProblemHttpResult>>
{
  public override void Configure()
  {
    Delete(DeleteBoardRequest.Route);
    Policies("RequireAuthenticatedUser");

    Summary(s =>
    {
      s.Summary = "Delete a board";
      s.Description = "Deletes an existing board by ID. This action cannot be undone.";
      s.ExampleRequest = new DeleteBoardRequest { BoardId = 1 };

      s.Responses[204] = "Board deleted successfully";
      s.Responses[401] = "Authentication required";
      s.Responses[404] = "Board not found";
      s.Responses[400] = "Invalid request or deletion failed";
    });

    Tags("Boards");

    Description(builder => builder
      .Accepts<DeleteBoardRequest>()
      .Produces(204)
      .ProducesProblem(401)
      .ProducesProblem(404)
      .ProducesProblem(400));
  }

  public override async Task<Results<NoContent, NotFound, ProblemHttpResult>>
    ExecuteAsync(DeleteBoardRequest req, CancellationToken ct)
  {
    if (!HttpContext.TryGetUserId(out var userId))
    {
      return TypedResults.Problem(title: "Unauthorized", statusCode: StatusCodes.Status401Unauthorized);
    }

    var cmd = new DeleteBoardCommand(BoardId.From(req.BoardId), userId);
    var result = await mediator.Send(cmd, ct);

    return result.ToDeleteResult();
  }
}
