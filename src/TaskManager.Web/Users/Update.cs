using TaskManager.Core.UserAggregate;
using TaskManager.UseCases.Users.Update;
using TaskManager.Web.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TaskManager.Web.Users;

public class Update(IMediator mediator)
  : Endpoint<UpdateUserRequest,
             Results<Ok<UpdateUserResponse>, NotFound, ProblemHttpResult>>
{
  public override void Configure()
  {
    Put(UpdateUserRequest.Route);
    Policies("RequireAuthenticatedUser");

    Summary(s =>
    {
      s.Summary = "Update a user";
      s.Description = "Updates user details. All fields are optional - only provided fields will be updated.";
      s.ExampleRequest = new UpdateUserRequest
      {
        UserId = 1,
        Name = "Jane Doe",
        Email = "jane.doe@gmail.com"
      };

      s.Responses[200] = "User updated successfully";
      s.Responses[401] = "Authentication required";
      s.Responses[404] = "User not found";
      s.Responses[400] = "Validation error";
    });

    Tags("Users");
  }

  public override async Task<Results<Ok<UpdateUserResponse>, NotFound, ProblemHttpResult>>
    ExecuteAsync(UpdateUserRequest request, CancellationToken cancellationToken)
  {
    var command = new UpdateUserCommand(
      request.UserId,
      string.IsNullOrEmpty(request.Name) ? null : UserName.From(request.Name),
      string.IsNullOrEmpty(request.Email) ? null : UserEmail.From(request.Email),
      request.Password);

    var result = await mediator.Send(command, cancellationToken);

    return result.ToUpdateResult(id => new UpdateUserResponse(id.Value));
  }
}
