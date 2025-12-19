using FluentValidation;
using TaskManager.Core.BoardAggregate;

namespace TaskManager.Web.Cards;

public class CreateCardValidator : Validator<CreateCardRequest>
{
  public CreateCardValidator()
  {
    RuleFor(x => x.Title)
      .NotEmpty()
      .WithMessage("Title is required.")
      .MinimumLength(2)
      .MaximumLength(CardTitle.MaxLength);

    When(x => !string.IsNullOrEmpty(x.Description), () =>
    {
      RuleFor(x => x.Description)
        .MaximumLength(CardDescription.MaxLength);
    });
    
    RuleFor(x => x.ColumnId)
      .GreaterThan(0)
      .WithMessage("ColumnId must be greater than 0.");

    RuleFor(x => x.BoardId)
      .GreaterThan(0)
      .WithMessage("BoardId must be greater than 0.");
  }
}
