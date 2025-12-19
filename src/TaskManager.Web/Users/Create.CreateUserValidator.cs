using FluentValidation;
using TaskManager.Core.UserAggregate;

namespace TaskManager.Web.Users;

public class CreateUserValidator : Validator<CreateUserRequest>
{
  public CreateUserValidator()
  {
    RuleFor(x => x.Name)
      .NotEmpty()
      .WithMessage("Name is required.")
      .MinimumLength(2)
      .MaximumLength(UserName.MaxLength);

    RuleFor(x => x.Email)
      .NotEmpty()
      .WithMessage("Email is required.")
      .MaximumLength(UserEmail.MaxLength);

    RuleFor(x => x.Password)
      .NotEmpty()
      .WithMessage("Password is required.")
      .MaximumLength(UserPassword.MaxLength)
      .Must(UserPassword.IsStrongPassword)
      .WithMessage("Password must be strong: minimum 8 characters, with upper/lowercase letters, digits, and special characters.");
  }
}
