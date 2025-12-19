using FluentValidation;
using TaskManager.Core.UserAggregate;

namespace TaskManager.Web.Users;

public class UpdateUserValidator : Validator<UpdateUserRequest>
{
  public UpdateUserValidator()
  {
    RuleFor(x => x.UserId)
      .GreaterThan(0)
      .WithMessage("UserId must be greater than 0.");

    When(x => !string.IsNullOrEmpty(x.Name), () =>
    {
      RuleFor(x => x.Name)
        .MinimumLength(2)
        .MaximumLength(UserName.MaxLength);
    });

    When(x => !string.IsNullOrEmpty(x.Email), () =>
    {
      RuleFor(x => x.Email)
        .MaximumLength(UserEmail.MaxLength);
    });

    When(x => !string.IsNullOrEmpty(x.Password), () =>
    {
      RuleFor(x => x.Password)
        .MaximumLength(UserPassword.MaxLength);
    });
  }
}
