using FluentValidation;
using TaskManager.Core.BoardAggregate;

namespace TaskManager.Web.Boards;

public class CreateBoardValidator : Validator<CreateBoardRequest>
{
  public CreateBoardValidator()
  {
    RuleFor(x => x.Name)
      .NotEmpty()
      .WithMessage("Name is required.")
      .MinimumLength(2)
      .MaximumLength(BoardName.MaxLength);
  }
}
