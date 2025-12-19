using FluentValidation;

namespace TaskManager.Web.Cards;

public sealed class ListCardsValidator : Validator<ListCardsRequest>
{
  public ListCardsValidator()
  {
    RuleFor(x => x.BoardId)
      .GreaterThan(0)
      .WithMessage("BoardId must be greater than 0.");

    RuleFor(x => x.ColumnId)
      .GreaterThan(0)
      .WithMessage("ColumnId must be greater than 0.");

    RuleFor(x => x.Page)
      .GreaterThanOrEqualTo(1)
      .WithMessage("page must be >= 1");

    RuleFor(x => x.PerPage)
      .InclusiveBetween(1, UseCases.Constants.MAX_PAGE_SIZE)
      .WithMessage($"per_page must be between 1 and {UseCases.Constants.MAX_PAGE_SIZE}");
  }
}
