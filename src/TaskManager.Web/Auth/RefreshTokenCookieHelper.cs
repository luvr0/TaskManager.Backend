using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace TaskManager.Web.Auth;

internal static class RefreshTokenCookieHelper
{
  public const string CookieName = "refreshToken";

  public static void AppendRefreshTokenCookie(
    HttpContext context,
    IWebHostEnvironment environment,
    string refreshToken,
    DateTime expiresAt)
  {
    var options = BuildCookieOptions(context, environment, expiresAt);
    context.Response.Cookies.Append(CookieName, refreshToken, options);
  }

  public static void DeleteRefreshTokenCookie(
    HttpContext context,
    IWebHostEnvironment environment)
  {
    var options = BuildBaseCookieOptions(context, environment);
    options.Expires = DateTimeOffset.UnixEpoch;
    context.Response.Cookies.Delete(CookieName, options);
  }

  public static bool TryReadRefreshToken(HttpContext context, out string? refreshToken)
  {
    if (context.Request.Cookies.TryGetValue(CookieName, out var token) && !string.IsNullOrWhiteSpace(token))
    {
      refreshToken = token;
      return true;
    }

    refreshToken = null;
    return false;
  }

  private static CookieOptions BuildCookieOptions(
    HttpContext context,
    IWebHostEnvironment environment,
    DateTime expiresAt)
  {
    var options = BuildBaseCookieOptions(context, environment);
    options.Expires = new DateTimeOffset(DateTime.SpecifyKind(expiresAt, DateTimeKind.Utc));
    return options;
  }

  private static CookieOptions BuildBaseCookieOptions(
    HttpContext context,
    IWebHostEnvironment environment)
  {
    return new CookieOptions
    {
      HttpOnly = true,
      Secure = !environment.IsDevelopment() || context.Request.IsHttps,
      SameSite = SameSiteMode.None,
      Path = "/"
    };
  }
}
