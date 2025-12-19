using FluentValidation;
using TaskManager.Core.BoardAggregate;

namespace TaskManager.Web.Boards;

public class UpdateBoardMemberRoleValidator : Validator<UpdateBoardMemberRoleRequest>
{
  public UpdateBoardMemberRoleValidator()
  {
    RuleFor(x => x.BoardId)
      .GreaterThan(0)
      .WithMessage("BoardId must be greater than 0.");

    RuleFor(x => x.MemberId)
      .GreaterThan(0)
      .WithMessage("MemberId must be greater than 0.");

    RuleFor(x => x.Role)
      .NotEmpty()
      .Must(BoardRole.IsValid)
      .WithMessage($"Role must be one of: {string.Join(", ", BoardRole.GetAllBoardRoles().Select(r => r.Value))}");
  }
}
