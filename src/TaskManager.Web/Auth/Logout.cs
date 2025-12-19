using Microsoft.AspNetCore.Http.HttpResults;
using TaskManager.UseCases.Authentication.Logout;

namespace TaskManager.Web.Auth;

public class Logout(IMediator mediator, IWebHostEnvironment environment)
  : Endpoint<LogoutRequest, Results<NoContent, ValidationProblem, ProblemHttpResult>>
{
  public override void Configure()
  {
    Post(LogoutRequest.Route);
    AllowAnonymous();

    Summary(s =>
    {
      s.Summary = "Logout user";
      s.Description = "Revokes the active refresh token (body or cookie) and removes the client cookie.";

      s.Responses[204] = "Logout successful";
      s.Responses[400] = "Validation error or logout failed";
    });

    Tags("Authentication");
  }

  public override async Task<Results<NoContent, ValidationProblem, ProblemHttpResult>>
    ExecuteAsync(LogoutRequest request, CancellationToken cancellationToken)
  {
    var refreshToken = ResolveRefreshToken(request);

    if (string.IsNullOrWhiteSpace(refreshToken))
    {
      return ReturnNoContent();
    }

    var result = await mediator.Send(new LogoutCommand(refreshToken), cancellationToken);

    return result.Status switch
    {
      ResultStatus.Ok => ReturnNoContent(),
      ResultStatus.Invalid => TypedResults.ValidationProblem(BuildValidationErrors(result)),
      _ => TypedResults.Problem(
        title: "Logout failed",
        detail: string.Join("; ", result.Errors),
        statusCode: StatusCodes.Status400BadRequest)
    };
  }

  private Results<NoContent, ValidationProblem, ProblemHttpResult> ReturnNoContent()
  {
    RefreshTokenCookieHelper.DeleteRefreshTokenCookie(HttpContext, environment);
    return TypedResults.NoContent();
  }

  private static Dictionary<string, string[]> BuildValidationErrors(Result result)
  {
    return result.ValidationErrors
      .GroupBy(e => e.Identifier ?? string.Empty)
      .ToDictionary(
        g => g.Key,
        g => g.Select(e => e.ErrorMessage).ToArray());
  }

  private string? ResolveRefreshToken(LogoutRequest request)
  {
    if (!string.IsNullOrWhiteSpace(request.RefreshToken))
    {
      return request.RefreshToken;
    }

    return RefreshTokenCookieHelper.TryReadRefreshToken(HttpContext, out var cookieToken)
      ? cookieToken
      : null;
  }
}
