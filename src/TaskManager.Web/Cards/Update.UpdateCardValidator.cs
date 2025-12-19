using FluentValidation;
using TaskManager.Core.BoardAggregate;

namespace TaskManager.Web.Cards;

public class UpdateCardValidator : Validator<UpdateCardRequest>
{
  public UpdateCardValidator()
  {
    When(x => !string.IsNullOrEmpty(x.Title), () =>
    {
      RuleFor(x => x.Title)
        .MinimumLength(2)
        .MaximumLength(CardTitle.MaxLength);
    });

    When(x => !string.IsNullOrEmpty(x.Description), () =>
    {
      RuleFor(x => x.Description)
        .MaximumLength(CardDescription.MaxLength);
    });

    When(x => !string.IsNullOrEmpty(x.Status), () =>
    {
      RuleFor(x => x.Status)
        .Must(s => CardStatus.TryFromName(s, ignoreCase: true, out _))
        .WithMessage("Status must be a valid CardStatus value (Pending, Done, Incomplete).");
    });
    
    RuleFor(x => x.CardId)
      .GreaterThan(0)
      .WithMessage("CardId must be greater than 0.");

    RuleFor(x => x.BoardId)
      .GreaterThan(0)
      .WithMessage("BoardId must be greater than 0.");
  }
}
