using FluentValidation;

namespace TaskManager.Web.Users;

public class GetUserByIdValidator : Validator<GetUserByIdRequest>
{
  public GetUserByIdValidator()
  {
    RuleFor(x => x.UserId)
      .GreaterThan(0)
      .WithMessage("UserId must be greater than 0.");
  }
}
