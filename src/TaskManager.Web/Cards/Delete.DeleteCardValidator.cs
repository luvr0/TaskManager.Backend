using FluentValidation;

namespace TaskManager.Web.Cards;

public class DeleteCardValidator : Validator<DeleteCardRequest>
{
  public DeleteCardValidator()
  {
    RuleFor(x => x.CardId)
      .GreaterThan(0)
      .WithMessage("CardId must be greater than 0.");

    RuleFor(x => x.BoardId)
      .GreaterThan(0)
      .WithMessage("BoardId must be greater than 0.");
  }
}
