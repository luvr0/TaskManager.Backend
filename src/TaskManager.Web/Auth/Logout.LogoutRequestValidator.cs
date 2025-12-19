using FluentValidation;
using TaskManager.Core.UserAggregate;

namespace TaskManager.Web.Auth;

public sealed class LogoutRequestValidator : Validator<LogoutRequest>
{
  public LogoutRequestValidator()
  {
    RuleFor(x => x.RefreshToken)
      .MaximumLength(RefreshToken.MaxLength)
      .When(x => !string.IsNullOrWhiteSpace(x.RefreshToken));
  }
}
