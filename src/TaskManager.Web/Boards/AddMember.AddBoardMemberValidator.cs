using FluentValidation;
using TaskManager.Core.BoardAggregate;

namespace TaskManager.Web.Boards;

public class AddBoardMemberValidator : Validator<AddBoardMemberRequest>
{
  public AddBoardMemberValidator()
  {
    RuleFor(x => x.BoardId)
      .GreaterThan(0)
      .WithMessage("BoardId must be greater than 0.");

    RuleFor(x => x.Email)
      .NotEmpty()
      .EmailAddress();

    RuleFor(x => x.Role)
      .NotEmpty()
      .Must(BoardRole.IsValid)
      .WithMessage($"Role must be one of: {string.Join(", ", BoardRole.GetAllBoardRoles().Select(r => r.Value))}");
  }
}
