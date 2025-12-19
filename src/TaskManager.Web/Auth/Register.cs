using Microsoft.AspNetCore.Http.HttpResults;
using TaskManager.Core.UserAggregate;
using TaskManager.UseCases.Users.Create;
using TaskManager.Web.Extensions;

namespace TaskManager.Web.Auth;

public class Register(IMediator mediator)
  : Endpoint<RegisterRequest,
             Results<Created<RegisterResponse>,
                     ValidationProblem,
                     ProblemHttpResult>>
{
  public override void Configure()
  {
    Post(RegisterRequest.Route);
    AllowAnonymous();
    Summary(s =>
    {
      s.Summary = "Register user";
      s.Description = "Creates a new user account with default Reader permissions.";
      s.ExampleRequest = new RegisterRequest
      {
        Name = "John Doe",
        Email = "john.doe@gmail.com",
        Password = "SecureP@ssw0rd"
      };
    });
    Tags("Authentication");
  }

  public override async Task<Results<Created<RegisterResponse>, ValidationProblem, ProblemHttpResult>>
    ExecuteAsync(RegisterRequest request, CancellationToken cancellationToken)
  {
    var command = new CreateUserCommand(
      UserName.From(request.Name!),
      UserEmail.From(request.Email!),
      request.Password!);

    var result = await mediator.Send(command, cancellationToken);

    return result.ToCreatedResult(
      id => $"/users/{id.Value}",
      id => new RegisterResponse(id.Value, request.Name!, request.Email!));
  }
}
