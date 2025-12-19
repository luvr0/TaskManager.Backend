using FluentValidation;

namespace TaskManager.Web.Columns;

public class GetColumnValidator : Validator<GetColumnByIdRequest>
{
  public GetColumnValidator()
  {
    RuleFor(x => x.ColumnId)
      .GreaterThan(0)
      .WithMessage("ColumnId must be greater than 0.");

    RuleFor(x => x.BoardId)
      .GreaterThan(0)
      .WithMessage("BoardId must be greater than 0.");
  }
}
