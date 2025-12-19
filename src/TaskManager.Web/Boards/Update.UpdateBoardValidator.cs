using FluentValidation;
using TaskManager.Core.BoardAggregate;

namespace TaskManager.Web.Boards;

/// <summary>
/// See: https://fast-endpoints.com/docs/validation
/// </summary>
public class UpdateBoardValidator : Validator<UpdateBoardRequest>
{
  public UpdateBoardValidator()
  {
    RuleFor(x => x.BoardId)
      .GreaterThan(0);

    RuleFor(x => x.Id)
      .GreaterThan(0);

    RuleFor(x => x.Name)
      .NotEmpty()
      .WithMessage("Name is required.")
      .MinimumLength(2)
      .MaximumLength(BoardName.MaxLength);
  }
}
