using Microsoft.AspNetCore.Http;
using TaskManager.Core.BoardAggregate;
using TaskManager.Core.UserAggregate;
using TaskManager.UseCases.Boards.RemoveMember;
using TaskManager.Web.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TaskManager.Web.Boards;

public class RemoveMember(IMediator mediator)
  : Endpoint<RemoveBoardMemberRequest, Results<NoContent, ValidationProblem, NotFound, ProblemHttpResult>>
{
  public override void Configure()
  {
    Delete(RemoveBoardMemberRequest.Route);
    Policies("RequireAuthenticatedUser");

    Summary(s =>
    {
      s.Summary = "Remove a member from a board";
      s.Description = "Removes a member by user id from the specified board.";
      s.ExampleRequest = new RemoveBoardMemberRequest { BoardId = 1, MemberId = 2 };

      s.Responses[204] = "Member removed successfully";
      s.Responses[401] = "Authentication required";
      s.Responses[404] = "Board not found";
      s.Responses[400] = "Validation error";
    });

    Tags("Boards");
  }

  public override async Task<Results<NoContent, ValidationProblem, NotFound, ProblemHttpResult>> ExecuteAsync(RemoveBoardMemberRequest req, CancellationToken ct)
  {
    if (!HttpContext.TryGetUserId(out var userId))
    {
      return TypedResults.Problem(title: "Unauthorized", statusCode: StatusCodes.Status401Unauthorized);
    }

    var cmd = new RemoveBoardMemberCommand(
      BoardId.From(req.BoardId),
      UserId.From(req.MemberId),
      userId);
    var result = await mediator.Send(cmd, ct);
    return result.ToNoContentResult();
  }
}
