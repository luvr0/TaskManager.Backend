using FluentValidation;

namespace TaskManager.Web.Cards;

public class MoveCardValidator : Validator<MoveCardRequest>
{
  public MoveCardValidator()
  {
    RuleFor(x => x.CardId)
      .GreaterThan(0)
      .WithMessage("CardId must be greater than 0.");

    RuleFor(x => x.BoardId)
      .GreaterThan(0)
      .WithMessage("BoardId must be greater than 0.");

    RuleFor(x => x.TargetColumnId)
      .GreaterThan(0)
      .WithMessage("TargetColumnId must be greater than 0.");
  }
}
