using Microsoft.AspNetCore.Http.HttpResults;
using TaskManager.Core.UserAggregate;
using TaskManager.UseCases.Authentication.Login;

namespace TaskManager.Web.Auth;

public class Login(IMediator mediator, IWebHostEnvironment environment)
  : Endpoint<LoginRequest,
             Results<Ok<AuthResponse>, ValidationProblem, UnauthorizedHttpResult, ProblemHttpResult>>
{
  public override void Configure()
  {
    Post(LoginRequest.Route);
    AllowAnonymous();

    Summary(s =>
    {
      s.Summary = "Authenticate user";
      s.Description = "Authenticates a user using email and password and returns a JWT plus refresh metadata.";
      s.ExampleRequest = new LoginRequest
      {
        Email = "john.doe@gmail.com",
        Password = "SecureP@ssw0rd"
      };

      s.Responses[200] = "Authentication successful";
      s.Responses[401] = "Invalid credentials";
      s.Responses[400] = "Validation error or user inactive";
    });

    Tags("Authentication");
  }

  public override async Task<Results<Ok<AuthResponse>, ValidationProblem, UnauthorizedHttpResult, ProblemHttpResult>>
    ExecuteAsync(LoginRequest request, CancellationToken cancellationToken)
  {
    var command = new AuthenticateUserCommand(
      UserEmail.From(request.Email!),
      request.Password!);

    var result = await mediator.Send(command, cancellationToken);

    if (result.Status == ResultStatus.Ok)
    {
      RefreshTokenCookieHelper.AppendRefreshTokenCookie(
        HttpContext,
        environment,
        result.Value.RefreshToken,
        result.Value.RefreshTokenExpiresAt);
    }

    return result.ToAuthEndpointResult();
  }
}
