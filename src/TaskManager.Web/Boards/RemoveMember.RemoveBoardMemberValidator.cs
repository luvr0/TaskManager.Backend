using FluentValidation;

namespace TaskManager.Web.Boards;

public class RemoveBoardMemberValidator : Validator<RemoveBoardMemberRequest>
{
  public RemoveBoardMemberValidator()
  {
    RuleFor(x => x.BoardId).GreaterThan(0).WithMessage("BoardId must be greater than 0.");
    RuleFor(x => x.MemberId).GreaterThan(0).WithMessage("MemberId must be greater than 0.");
  }
}
