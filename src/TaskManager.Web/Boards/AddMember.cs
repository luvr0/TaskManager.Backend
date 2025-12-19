using Microsoft.AspNetCore.Http;
using TaskManager.Core.BoardAggregate;
using TaskManager.Core.UserAggregate;
using TaskManager.UseCases.Boards.AddMember;
using TaskManager.Web.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TaskManager.Web.Boards;

public class AddMember(IMediator mediator)
  : Endpoint<AddBoardMemberRequest,
             Results<Ok<AddBoardMemberResponse>, ValidationProblem, NotFound, ProblemHttpResult>>
{
  public override void Configure()
  {
    Post(AddBoardMemberRequest.Route);
    Policies("RequireAuthenticatedUser");

    Summary(summary =>
    {
      summary.Summary = "Add a member to a board";
      summary.Description = "Adds an existing user to a board using their email and assigns a board-specific role.";
      summary.ExampleRequest = new AddBoardMemberRequest { BoardId = 1, Email = "editor@gmail.com", Role = BoardRole.Editor.Value };
      summary.ResponseExamples[200] = new AddBoardMemberResponse(1, new BoardMemberRecord(2, "editor@gmail.com", BoardRole.Editor.Value));

      summary.Responses[200] = "Member added to board";
      summary.Responses[401] = "Authentication required";
      summary.Responses[404] = "Board not found";
      summary.Responses[400] = "Validation error";
    });

    Tags("Boards");
  }

  public override async Task<Results<Ok<AddBoardMemberResponse>, ValidationProblem, NotFound, ProblemHttpResult>>
    ExecuteAsync(AddBoardMemberRequest request, CancellationToken ct)
  {
    if (!HttpContext.TryGetUserId(out var userId))
    {
      return TypedResults.Problem(title: "Unauthorized", statusCode: StatusCodes.Status401Unauthorized);
    }

    var command = new AddBoardMemberCommand(
      BoardId.From(request.BoardId),
      UserEmail.From(request.Email),
      BoardRole.From(request.Role),
      userId);

    var result = await mediator.Send(command, ct);

    return result.ToOkWithNotFoundResult(dto => new AddBoardMemberResponse(
      dto.BoardId.Value,
      new BoardMemberRecord(dto.UserId.Value, dto.Email.Value, dto.Role.Value)));
  }
}
