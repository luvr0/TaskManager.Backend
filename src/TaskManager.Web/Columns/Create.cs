using TaskManager.Core.BoardAggregate;
using TaskManager.UseCases.Columns.Create;
using TaskManager.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TaskManager.Web.Columns;

public class Create(IMediator mediator)
  : Endpoint<CreateColumnRequest,
          Results<Created<CreateColumnResponse>,
                          ValidationProblem,
                          ProblemHttpResult>>
{
  public override void Configure()
  {
    Post(CreateColumnRequest.Route);
    Policies("RequireAuthenticatedUser");

    Summary(s =>
    {
      s.Summary = "Create a new column in a board";
      s.Description = "Creates a new column with the provided name within a specific board.";
      s.ExampleRequest = new CreateColumnRequest { Name = "To Do", BoardId = 1 };
      s.ResponseExamples[201] = new CreateColumnResponse(1, "To Do", 1);

      s.Responses[201] = "Column created successfully";
      s.Responses[401] = "Authentication required";
      s.Responses[400] = "Invalid input data - validation errors";
      s.Responses[404] = "Board not found";
    });

    Tags("Columns");

    Description(builder => builder
      .Accepts<CreateColumnRequest>("application/json")
      .Produces<CreateColumnResponse>(201, "application/json")
      .ProducesProblem(401)
      .ProducesProblem(400)
      .ProducesProblem(404));
  }

  public override async Task<Results<Created<CreateColumnResponse>, ValidationProblem, ProblemHttpResult>>
    ExecuteAsync(CreateColumnRequest request, CancellationToken cancellationToken)
  {
    if (!HttpContext.TryGetUserId(out var userId))
    {
      return TypedResults.Problem(
        title: "Unauthorized",
        detail: "User context could not be resolved.",
        statusCode: StatusCodes.Status401Unauthorized);
    }

    var result = await mediator.Send(
      new CreateColumnCommand(
        ColumnName.From(request.Name!),
        BoardId.From(request.BoardId),
        userId),
      cancellationToken);

    return result.ToCreatedResult(
      id => $"/Boards/{request.BoardId}/Columns/{id}",
      id => new CreateColumnResponse(id.Value, request.Name!, request.BoardId));
  }
}
