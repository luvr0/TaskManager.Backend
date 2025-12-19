using TaskManager.Core.BoardAggregate;
using TaskManager.UseCases.Boards;
using TaskManager.UseCases.Boards.Update;
using TaskManager.Web.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TaskManager.Web.Boards;

public class Update(IMediator mediator)
  : Endpoint<UpdateBoardRequest,
             Results<Ok<UpdateBoardResponse>, NotFound, ProblemHttpResult>,
             UpdateBoardMapper>
{
  public override void Configure()
  {
    Put(UpdateBoardRequest.Route);
    Policies("RequireAuthenticatedUser");

    Summary(s =>
    {
      s.Summary = "Update a board";
      s.Description = "Updates an existing board's information. The board name must be between 2 and 100 characters long.";
      s.ExampleRequest = new UpdateBoardRequest { Id = 1, Name = "Updated Name" };
      s.ResponseExamples[200] = new UpdateBoardResponse(
        new BoardRecord(1, "Updated Name", 1, 0, new List<MemberSummaryResponse>()));

      // Document possible responses
      s.Responses[200] = "Board updated successfully";
      s.Responses[401] = "Authentication required";
      s.Responses[404] = "Board with specified ID not found";
      s.Responses[400] = "Invalid input data or business rule violation";
    });

    // Add tags for API grouping
    Tags("Boards");

    // Add additional metadata
    Description(builder => builder
      .Accepts<UpdateBoardRequest>("application/json")
      .Produces<UpdateBoardResponse>(200, "application/json")
      .ProducesProblem(401)
      .ProducesProblem(404)
      .ProducesProblem(400));
  }

  public override async Task<Results<Ok<UpdateBoardResponse>, NotFound, ProblemHttpResult>>
    ExecuteAsync(UpdateBoardRequest request, CancellationToken ct)
  {
    if (!HttpContext.TryGetUserId(out var userId))
    {
      return TypedResults.Problem(title: "Unauthorized", statusCode: StatusCodes.Status401Unauthorized);
    }

    var cmd = new UpdateBoardCommand(
      BoardId.From(request.Id),
      BoardName.From(request.Name!),
      userId);

    var result = await mediator.Send(cmd, ct);

    return result.ToUpdateResult(Map.FromEntity);
  }
}

public sealed class UpdateBoardMapper
  : Mapper<UpdateBoardRequest, UpdateBoardResponse, BoardDto>
{
  public override UpdateBoardResponse FromEntity(BoardDto e)
  {
    var members = e.Members
      .Select(m => new MemberSummaryResponse(m.Id.Value, m.Role.Value, m.Email, m.EmailAlias))
      .ToList();

    return new UpdateBoardResponse(
      new BoardRecord(e.Id.Value, e.Name.Value, e.OwnerId.Value, members.Count, members));
  }
}
