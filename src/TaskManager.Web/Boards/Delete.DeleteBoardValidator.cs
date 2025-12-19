using FluentValidation;

namespace TaskManager.Web.Boards;

/// <summary>
/// See: https://fast-endpoints.com/docs/validation
/// </summary>
public class DeleteBoardValidator : Validator<DeleteBoardRequest>
{
  public DeleteBoardValidator()
  {
    RuleFor(x => x.BoardId)
      .GreaterThan(0);
  }
}
