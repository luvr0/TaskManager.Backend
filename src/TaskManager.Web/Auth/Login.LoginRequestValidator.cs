using FluentValidation;

namespace TaskManager.Web.Auth;

public class LoginRequestValidator : Validator<LoginRequest>
{
  public LoginRequestValidator()
  {
    RuleFor(x => x.Email)
      .NotEmpty()
      .EmailAddress();

    RuleFor(x => x.Password)
      .NotEmpty();
  }
}
