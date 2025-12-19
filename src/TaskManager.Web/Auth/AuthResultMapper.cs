using System.Linq;
using Microsoft.AspNetCore.Http.HttpResults;
using TaskManager.UseCases.Authentication;

namespace TaskManager.Web.Auth;

internal static class AuthResultMapper
{
  public static Results<Ok<AuthResponse>, ValidationProblem, UnauthorizedHttpResult, ProblemHttpResult>
    ToAuthEndpointResult(this Result<AuthTokensDto> result)
  {
    return result.Status switch
    {
      ResultStatus.Ok => TypedResults.Ok(AuthResponse.FromDto(result.Value)),
      ResultStatus.Invalid => TypedResults.ValidationProblem(
        result.ValidationErrors
          .GroupBy(e => e.Identifier ?? string.Empty)
          .ToDictionary(
            g => g.Key,
            g => g.Select(e => e.ErrorMessage).ToArray()
          )
      ),
      ResultStatus.Unauthorized => TypedResults.Unauthorized(),
      _ => TypedResults.Problem(
        title: "Authentication failed",
        detail: string.Join("; ", result.Errors),
        statusCode: StatusCodes.Status400BadRequest)
    };
  }
}
