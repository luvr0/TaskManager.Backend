using TaskManager.Core.BoardAggregate;
using TaskManager.UseCases.Cards.Create;
using TaskManager.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TaskManager.Web.Cards;

public class Create(IMediator mediator)
  : Endpoint<CreateCardRequest,
             Results<Created<CreateCardResponse>, ValidationProblem, ProblemHttpResult>>
{
  public override void Configure()
  {
    Post(CreateCardRequest.Route);
    Policies("RequireAuthenticatedUser");

    Summary(s =>
    {
      s.Summary = "Create a new card in a column";
      s.Description = "Creates a new card with the provided title and optional description within a specific column.";
      s.ExampleRequest = new CreateCardRequest { Title = "Implement login", Description = "Add user authentication", ColumnId = 1, BoardId = 1 };
      s.ResponseExamples[201] = new CreateCardResponse(1, "Implement login", "Add user authentication", 1, 1);

      s.Responses[201] = "Card created successfully";
      s.Responses[401] = "Authentication required";
      s.Responses[400] = "Invalid input data - validation errors";
      s.Responses[404] = "Column or Board not found";
    });

    Tags("Cards");

    Description(builder => builder
      .Accepts<CreateCardRequest>("application/json")
      .Produces<CreateCardResponse>(201, "application/json")
      .ProducesProblem(401)
      .ProducesProblem(400)
      .ProducesProblem(404));
  }

  public override async Task<Results<Created<CreateCardResponse>, ValidationProblem, ProblemHttpResult>>
    ExecuteAsync(CreateCardRequest request, CancellationToken cancellationToken)
  {
    if (!HttpContext.TryGetUserId(out var userId))
    {
      return TypedResults.Problem(
        title: "Unauthorized",
        detail: "User context could not be resolved.",
        statusCode: StatusCodes.Status401Unauthorized);
    }

    CardDescription? description = string.IsNullOrEmpty(request.Description)
      ? null
      : CardDescription.From(request.Description);

    var result = await mediator.Send(
      new CreateCardCommand(
        CardTitle.From(request.Title!),
        description,
        ColumnId.From(request.ColumnId),
        BoardId.From(request.BoardId),
        userId),
      cancellationToken);

    return result.ToCreatedResult(
      id => $"/Boards/{request.BoardId}/Cards/{id}",
      id => new CreateCardResponse(id.Value, request.Title!, request.Description ?? string.Empty, request.ColumnId, request.BoardId));
  }
}
