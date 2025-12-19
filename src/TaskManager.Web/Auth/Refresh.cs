using Microsoft.AspNetCore.Http.HttpResults;
using TaskManager.UseCases.Authentication.Refresh;

namespace TaskManager.Web.Auth;

public class Refresh(IMediator mediator, IWebHostEnvironment environment)
  : Endpoint<RefreshTokenRequest,
             Results<Ok<AuthResponse>, ValidationProblem, UnauthorizedHttpResult, ProblemHttpResult>>
{
  public override void Configure()
  {
    Post(RefreshTokenRequest.Route);
    AllowAnonymous();

    Summary(s =>
    {
      s.Summary = "Refresh access token";
      s.Description = "Exchanges a valid refresh token (body or HttpOnly cookie) for a new access token.";
      s.ExampleRequest = new RefreshTokenRequest { RefreshToken = "<refresh-token>" };

      s.Responses[200] = "New tokens issued successfully";
      s.Responses[401] = "Invalid or expired refresh token";
      s.Responses[400] = "Validation error";
    });

    Tags("Authentication");
  }

  public override async Task<Results<Ok<AuthResponse>, ValidationProblem, UnauthorizedHttpResult, ProblemHttpResult>>
    ExecuteAsync(RefreshTokenRequest request, CancellationToken cancellationToken)
  {
    var refreshToken = ResolveRefreshToken(request);

    if (string.IsNullOrWhiteSpace(refreshToken))
    {
      return TypedResults.ValidationProblem(new Dictionary<string, string[]>
      {
        ["refreshToken"] = ["Refresh token must be provided in the body or cookie."]
      });
    }

    var result = await mediator.Send(new RefreshTokenCommand(refreshToken), cancellationToken);

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

  private string? ResolveRefreshToken(RefreshTokenRequest request)
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
