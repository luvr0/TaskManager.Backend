using TaskManager.UseCases.Users.Delete;
using TaskManager.Web.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TaskManager.Web.Users;

public class Delete(IMediator mediator)
  : Endpoint<DeleteUserRequest,
             Results<NoContent, ValidationProblem, NotFound, ProblemHttpResult>>
{
  public override void Configure()
  {
    Delete(DeleteUserRequest.Route);
    Policies("RequireAuthenticatedUser");

    Summary(s =>
    {
      s.Summary = "Delete a user";
      s.Description = "Permanently deletes a user from the system. Requires authentication.";

      s.Responses[204] = "User deleted successfully";
      s.Responses[401] = "Authentication required";
      s.Responses[404] = "User not found";
    });

    Tags("Users");
  }

  public override async Task<Results<NoContent, ValidationProblem, NotFound, ProblemHttpResult>>
    ExecuteAsync(DeleteUserRequest request, CancellationToken cancellationToken)
  {
    var result = await mediator.Send(
      new DeleteUserCommand(request.UserId),
      cancellationToken);

    return result.ToNoContentResult();
  }
}
