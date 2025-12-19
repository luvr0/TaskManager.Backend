using TaskManager.Core.UserAggregate;
using TaskManager.UseCases.Users.Get;
using TaskManager.Web.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TaskManager.Web.Users;

public class GetById(IMediator mediator)
  : Endpoint<GetUserByIdRequest,
             Results<Ok<UserDetailResponse>, NotFound, ProblemHttpResult>>
{
  public override void Configure()
  {
    Get(GetUserByIdRequest.Route);
    Policies("RequireAuthenticatedUser");

    Summary(s =>
    {
      s.Summary = "Get user by ID";
      s.Description = "Retrieves user details by their unique identifier";

      s.Responses[200] = "User found and returned successfully";
      s.Responses[401] = "Authentication required";
      s.Responses[404] = "User not found";
    });

    Tags("Users");
  }

  public override async Task<Results<Ok<UserDetailResponse>, NotFound, ProblemHttpResult>>
    ExecuteAsync(GetUserByIdRequest request, CancellationToken cancellationToken)
  {
    var result = await mediator.Send(
      new GetUserQuery(UserId.From(request.UserId)),
      cancellationToken);

    return result.ToGetByIdResult(user => new UserDetailResponse(
      user.Id,
      user.Name,
      user.Email));
  }
}
