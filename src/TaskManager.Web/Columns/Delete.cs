using TaskManager.Core.BoardAggregate;
using TaskManager.UseCases.Columns.Delete;
using TaskManager.Web.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TaskManager.Web.Columns;

public class Delete(IMediator mediator)
  : Endpoint<DeleteColumnRequest, Results<NoContent, NotFound, ProblemHttpResult>>
{
  public override void Configure()
  {
    Delete(DeleteColumnRequest.Route);
    Policies("RequireAuthenticatedUser");

    Summary(s =>
    {
      s.Summary = "Delete a column";
      s.Description = "Deletes an existing column by ID. This action cannot be undone and will also delete all cards in the column.";
      s.ExampleRequest = new DeleteColumnRequest { ColumnId = 1, BoardId = 1 };

      s.Responses[204] = "Column deleted successfully";
      s.Responses[401] = "Authentication required";
      s.Responses[404] = "Column or Board not found";
      s.Responses[400] = "Invalid request or deletion failed";
    });

    Tags("Columns");

    Description(builder => builder
      .Accepts<DeleteColumnRequest>()
      .Produces(204)
      .ProducesProblem(401)
      .ProducesProblem(404)
      .ProducesProblem(400));
  }

  public override async Task<Results<NoContent, NotFound, ProblemHttpResult>>
    ExecuteAsync(DeleteColumnRequest req, CancellationToken ct)
  {
    if (!HttpContext.TryGetUserId(out var userId))
    {
      return TypedResults.Problem(title: "Unauthorized", statusCode: StatusCodes.Status401Unauthorized);
    }

    var cmd = new DeleteColumnCommand(
      ColumnId.From(req.ColumnId),
      BoardId.From(req.BoardId),
      userId);

    var result = await mediator.Send(cmd, ct);

    return result.ToDeleteResult();
  }
}
