using TaskManager.Core.BoardAggregate;
using TaskManager.UseCases.Boards;
using TaskManager.UseCases.Boards.Get;
using TaskManager.Web.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TaskManager.Web.Boards;

public class GetById(IMediator mediator)
  : Endpoint<GetBoardByIdRequest,
             Results<Ok<BoardResponse>, NotFound, ProblemHttpResult>,
             GetBoardByIdMapper>
{
  public override void Configure()
  {
    Get(GetBoardByIdRequest.Route);
    Policies("RequireAuthenticatedUser");

    Summary(s =>
    {
      s.Summary = "Get a board by ID";
      s.Description = "Retrieves a specific board when the authenticated user is the owner or a member.";
      s.ExampleRequest = new GetBoardByIdRequest { BoardId = 1 };
      s.ResponseExamples[200] = new BoardResponse(1, "Project Alpha", 1, new List<MemberResponse>(), new List<ColumnResponse>());

      s.Responses[200] = "Board found and returned successfully";
      s.Responses[401] = "Authentication required";
      s.Responses[404] = "Board with specified ID not found";
    });

    Tags("Boards");

    Description(builder => builder
      .Accepts<GetBoardByIdRequest>()
      .Produces<BoardResponse>(200, "application/json")
      .ProducesProblem(401)
      .ProducesProblem(404));
  }

  public override async Task<Results<Ok<BoardResponse>, NotFound, ProblemHttpResult>>
    ExecuteAsync(GetBoardByIdRequest request, CancellationToken ct)
  {
    if (!HttpContext.TryGetUserId(out var userId))
    {
      return TypedResults.Problem(title: "Unauthorized", statusCode: StatusCodes.Status401Unauthorized);
    }

    var result = await mediator.Send(
      new GetBoardQuery(BoardId.From(request.BoardId), userId), ct);

    return result.ToGetByIdResult(Map.FromEntity);
  }
}

public sealed class GetBoardByIdMapper
  : Mapper<GetBoardByIdRequest, BoardResponse, BoardDto>
{
  public override BoardResponse FromEntity(BoardDto e)
  {
    var columns = e.Columns
      .Select(c => new ColumnResponse(
        c.Id.Value,
        c.Name.Value,
        c.Order.Value,
        c.Cards
          .Select(card => new CardResponse(
            card.Id.Value,
            card.Title.Value,
            card.Description?.Value ?? string.Empty,
            card.Order.Value,
            card.Status.ToString()))
          .ToList()))
      .ToList();

    var members = e.Members
      .Select(m => new MemberResponse(m.Id.Value, m.Role.Value))
      .ToList();

    return new BoardResponse(e.Id.Value, e.Name.Value, e.OwnerId.Value, members, columns);
  }
}
