using FluentValidation;
using TaskManager.Core.UserAggregate;

namespace TaskManager.Web.Auth;

public sealed class RegisterRequestValidator : Validator<RegisterRequest>
{
  public RegisterRequestValidator()
  {
    RuleFor(x => x.Name)
      .NotEmpty()
      .MinimumLength(2)
      .MaximumLength(UserName.MaxLength);

    RuleFor(x => x.Email)
      .NotEmpty()
      .EmailAddress()
      .MaximumLength(UserEmail.MaxLength);

    RuleFor(x => x.Password)
      .NotEmpty()
      .MaximumLength(UserPassword.MaxLength)
      .Must(UserPassword.IsStrongPassword)
      .WithMessage("Password must be strong: minimum 8 characters, with upper/lowercase letters, digits, and special characters.");
  }
}
