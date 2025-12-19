using TaskManager.Core.BoardAggregate;
using TaskManager.UseCases.Boards.Create;
using TaskManager.Web.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TaskManager.Web.Boards;

public class Create(IMediator mediator)
  : Endpoint<CreateBoardRequest,
             Results<Created<CreateBoardResponse>, ValidationProblem, ProblemHttpResult>>
{
  public override void Configure()
  {
    Post(CreateBoardRequest.Route);
    Policies("RequireAuthenticatedUser");

    Summary(s =>
    {
      s.Summary = "Create a new board";
      s.Description = "Creates a new board with the provided name. The authenticated user becomes the owner.";
      s.ExampleRequest = new CreateBoardRequest { Name = "Project Alpha" };
      s.ResponseExamples[201] = new CreateBoardResponse(1, "Project Alpha");

      s.Responses[201] = "Board created successfully";
      s.Responses[401] = "Authentication required";
      s.Responses[400] = "Invalid input data - validation errors";
      s.Responses[500] = "Internal server error";
    });

    Tags("Boards");

    Description(builder => builder
      .Accepts<CreateBoardRequest>("application/json")
      .Produces<CreateBoardResponse>(201, "application/json")
      .ProducesProblem(401)
      .ProducesProblem(400)
      .ProducesProblem(500));
  }

  public override async Task<Results<Created<CreateBoardResponse>, ValidationProblem, ProblemHttpResult>>
    ExecuteAsync(CreateBoardRequest request, CancellationToken cancellationToken)
  {
    if (!HttpContext.TryGetUserId(out var userId))
    {
      return TypedResults.Problem(title: "Unauthorized", statusCode: StatusCodes.Status401Unauthorized);
    }

    var result = await mediator.Send(
      new CreateBoardCommand(BoardName.From(request.Name!), userId),
      cancellationToken);

    return result.ToCreatedResult(
      id => $"/Boards/{id}",
      id => new CreateBoardResponse(id.Value, request.Name!));
  }
}
