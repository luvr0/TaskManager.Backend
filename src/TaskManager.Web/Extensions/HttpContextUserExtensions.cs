using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using TaskManager.Core.UserAggregate;

namespace TaskManager.Web.Extensions;

public static class HttpContextUserExtensions
{
  /// <summary>
  /// Attempts to extract the authenticated user's ID from the HTTP context claims.
  /// </summary>
  /// <param name="context">The HTTP context containing the user claims.</param>
  /// <param name="userId">When successful, contains the extracted UserId.</param>
  /// <returns>True if the UserId was successfully extracted; otherwise, false.</returns>
  public static bool TryGetUserId(this HttpContext context, out UserId userId)
  {
    if (context.User?.Identity?.IsAuthenticated == true)
    {
      var sub = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

      if (int.TryParse(sub, out var id))
      {
        userId = UserId.From(id);
        return true;
      }
    }

    Unsafe.SkipInit(out userId);
    return false;
  }

  /// <summary>
  /// Gets the authenticated user's ID or throws if not authenticated.
  /// </summary>
  /// <param name="context">The HTTP context containing the user claims.</param>
  /// <returns>The authenticated user's ID.</returns>
  /// <exception cref="UnauthorizedAccessException">Thrown when user is not authenticated.</exception>
  public static UserId GetRequiredUserId(this HttpContext context)
  {
    if (!TryGetUserId(context, out var userId))
    {
      throw new UnauthorizedAccessException("User is not authenticated.");
    }

    return userId;
  }
}
