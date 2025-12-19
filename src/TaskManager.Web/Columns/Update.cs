using TaskManager.Core.BoardAggregate;
using TaskManager.UseCases.Boards;
using TaskManager.UseCases.Columns.Update;
using TaskManager.Web.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TaskManager.Web.Columns;

public class Update(IMediator mediator)
  : Endpoint<UpdateColumnRequest,
             Results<Ok<UpdateColumnResponse>, NotFound, ProblemHttpResult>,
             UpdateColumnMapper>
{
  public override void Configure()
  {
    Put(UpdateColumnRequest.Route);
    Policies("RequireAuthenticatedUser");

    Summary(s =>
    {
      s.Summary = "Update a column";
      s.Description = "Updates an existing column's information.";
      s.ExampleRequest = new UpdateColumnRequest { ColumnId = 1, BoardId = 1, Name = "In Progress" };
      s.ResponseExamples[200] = new UpdateColumnResponse(new ColumnRecord(1, "In Progress", 1, 1));

      s.Responses[200] = "Column updated successfully";
      s.Responses[401] = "Authentication required";
      s.Responses[404] = "Column or Board not found";
      s.Responses[400] = "Invalid input data or business rule violation";
    });

    Tags("Columns");

    Description(builder => builder
      .Accepts<UpdateColumnRequest>("application/json")
      .Produces<UpdateColumnResponse>(200, "application/json")
      .ProducesProblem(401)
      .ProducesProblem(404)
      .ProducesProblem(400));
  }

  public override async Task<Results<Ok<UpdateColumnResponse>, NotFound, ProblemHttpResult>>
    ExecuteAsync(UpdateColumnRequest request, CancellationToken ct)
  {
    if (!HttpContext.TryGetUserId(out var userId))
    {
      return TypedResults.Problem(title: "Unauthorized", statusCode: StatusCodes.Status401Unauthorized);
    }

    var cmd = new UpdateColumnCommand(
      ColumnId.From(request.ColumnId),
      BoardId.From(request.BoardId),
      ColumnName.From(request.Name!),
      userId);

    var result = await mediator.Send(cmd, ct);

    return result.ToUpdateResult(Map.FromEntity);
  }
}

public sealed class UpdateColumnMapper
  : Mapper<UpdateColumnRequest, UpdateColumnResponse, ColumnDto>
{
  public override UpdateColumnResponse FromEntity(ColumnDto e)
    => new(new ColumnRecord(e.Id.Value, e.Name.Value, e.BoardId.Value, e.Order.Value));
}
