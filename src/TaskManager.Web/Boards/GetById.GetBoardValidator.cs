using FluentValidation;

namespace TaskManager.Web.Boards;

/// <summary>
/// See: https://fast-endpoints.com/docs/validation
/// </summary>
public class GetBoardValidator : Validator<GetBoardByIdRequest>
{
  public GetBoardValidator()
  {
    RuleFor(x => x.BoardId)
      .GreaterThan(0);
  }
}
