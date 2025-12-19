using FluentValidation;
using TaskManager.Core.UserAggregate;

namespace TaskManager.Web.Auth;

public class RefreshTokenRequestValidator : Validator<RefreshTokenRequest>
{
  public RefreshTokenRequestValidator()
  {
    RuleFor(x => x.RefreshToken)
      .MaximumLength(RefreshToken.MaxLength)
      .When(x => !string.IsNullOrWhiteSpace(x.RefreshToken));
  }
}
