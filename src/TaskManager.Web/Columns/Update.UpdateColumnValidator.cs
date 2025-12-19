using FluentValidation;
using TaskManager.Core.BoardAggregate;

namespace TaskManager.Web.Columns;

public class UpdateColumnValidator : Validator<UpdateColumnRequest>
{
  public UpdateColumnValidator()
  {
    RuleFor(x => x.Name)
      .NotEmpty()
      .WithMessage("Name is required.")
      .MinimumLength(2)
      .MaximumLength(ColumnName.MaxLength);
    
    RuleFor(x => x.ColumnId)
      .GreaterThan(0)
      .WithMessage("ColumnId must be greater than 0.");

    RuleFor(x => x.BoardId)
      .GreaterThan(0)
      .WithMessage("BoardId must be greater than 0.");
  }
}
