using FluentValidation;

namespace TaskManager.Web.Columns;

public class DeleteColumnValidator : Validator<DeleteColumnRequest>
{
  public DeleteColumnValidator()
  {
    RuleFor(x => x.ColumnId)
      .GreaterThan(0)
      .WithMessage("ColumnId must be greater than 0.");

    RuleFor(x => x.BoardId)
      .GreaterThan(0)
      .WithMessage("BoardId must be greater than 0.");
  }
}
