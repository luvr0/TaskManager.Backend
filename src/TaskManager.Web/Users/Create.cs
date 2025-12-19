using TaskManager.Core.UserAggregate;
using TaskManager.UseCases.Users.Create;
using TaskManager.Web.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TaskManager.Web.Users;

/// <summary>
/// Admin endpoint to create users.
/// For self-registration, use /auth/register instead.
/// </summary>
public class Create(IMediator mediator)
  : Endpoint<CreateUserRequest,
             Results<Created<CreateUserResponse>, ValidationProblem, ProblemHttpResult>>
{
  public override void Configure()
  {
    Post(CreateUserRequest.Route);
    Policies("RequireAuthenticatedUser");

    Summary(s =>
    {
      s.Summary = "Create a new user";
      s.Description = "Creates a new user account with the specified details. Requires authentication.";
      s.ExampleRequest = new CreateUserRequest
      {
        Name = "John Doe",
        Email = "john.doe@gmail.com",
        Password = "SecureP@ssw0rd"
      };

      s.Responses[201] = "User created successfully";
      s.Responses[401] = "Authentication required";
      s.Responses[400] = "Validation error";
    });

    Tags("Users");
  }

  public override async Task<Results<Created<CreateUserResponse>, ValidationProblem, ProblemHttpResult>>
    ExecuteAsync(CreateUserRequest request, CancellationToken cancellationToken)
  {
    var command = new CreateUserCommand(
      UserName.From(request.Name!),
      UserEmail.From(request.Email!),
      request.Password!);

    var result = await mediator.Send(command, cancellationToken);

    return result.ToCreatedResult(
      id => $"/users/{id.Value}",
      id => new CreateUserResponse(id.Value, request.Name!, request.Email!));
  }
}
