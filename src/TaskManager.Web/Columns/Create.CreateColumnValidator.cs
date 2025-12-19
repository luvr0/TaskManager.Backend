using FluentValidation;
using TaskManager.Core.BoardAggregate;

namespace TaskManager.Web.Columns;

public class CreateColumnValidator : Validator<CreateColumnRequest>
{
  public CreateColumnValidator()
  {
    RuleFor(x => x.Name)
      .NotEmpty()
      .WithMessage("Name is required.")
      .MinimumLength(2)
      .MaximumLength(ColumnName.MaxLength);
    
    RuleFor(x => x.BoardId)
      .GreaterThan(0)
      .WithMessage("BoardId must be greater than 0.");
  }
}
