using Microsoft.AspNetCore.Http;
using TaskManager.Core.BoardAggregate;
using TaskManager.Core.UserAggregate;
using TaskManager.UseCases.Boards.UpdateMemberRole;
using TaskManager.Web.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TaskManager.Web.Boards;

public class UpdateMemberRole(IMediator mediator)
  : Endpoint<UpdateBoardMemberRoleRequest,
             Results<Ok<UpdateBoardMemberRoleResponse>, ValidationProblem, NotFound, ProblemHttpResult>>
{
  public override void Configure()
  {
    Put(UpdateBoardMemberRoleRequest.Route);
    Policies("RequireAuthenticatedUser");

    Summary(summary =>
    {
      summary.Summary = "Update a board member role";
      summary.Description = "Updates the board-specific role assigned to an existing member.";
      summary.ExampleRequest = new UpdateBoardMemberRoleRequest
      {
        BoardId = 1,
        MemberId = 2,
        Role = BoardRole.Editor.Value
      };
      summary.ResponseExamples[200] = new UpdateBoardMemberRoleResponse(1,
        new BoardMemberRecord(2, "member@gmail.com", BoardRole.Editor.Value));

      summary.Responses[200] = "Member role updated successfully";
      summary.Responses[401] = "Authentication required";
      summary.Responses[404] = "Board not found";
      summary.Responses[400] = "Validation or processing error";
    });

    Tags("Boards");
  }

  public override async Task<Results<Ok<UpdateBoardMemberRoleResponse>, ValidationProblem, NotFound, ProblemHttpResult>>
    ExecuteAsync(UpdateBoardMemberRoleRequest request, CancellationToken ct)
  {
    if (!HttpContext.TryGetUserId(out var userId))
    {
      return TypedResults.Problem(title: "Unauthorized", statusCode: StatusCodes.Status401Unauthorized);
    }

    var command = new UpdateBoardMemberRoleCommand(
      BoardId.From(request.BoardId),
      UserId.From(request.MemberId),
      BoardRole.From(request.Role),
      userId);

    var result = await mediator.Send(command, ct);

    return result.ToOkWithNotFoundResult(dto => new UpdateBoardMemberRoleResponse(
      dto.BoardId.Value,
      new BoardMemberRecord(dto.UserId.Value, dto.Email.Value, dto.Role.Value)));
  }
}
