using Vogen;

namespace TaskManager.Core.BoardAggregate;

[ValueObject<string>(conversions: Conversions.SystemTextJson)]
public partial struct BoardRole
{
  public const int MaxLength = 50;

  public static BoardRole Manager => From("Manager");
  public static BoardRole Editor => From("Editor");
  public static BoardRole Reader => From("Reader");

  private static Validation Validate(in string role) =>
    string.IsNullOrWhiteSpace(role)
      ? Validation.Invalid("BoardRole cannot be empty")
      : role.Length > MaxLength
        ? Validation.Invalid($"BoardRole cannot be longer than {MaxLength} characters")
        : Validation.Ok;

  public static IReadOnlyList<BoardRole> GetAllBoardRoles() => new List<BoardRole> { Manager, Editor, Reader };

  public static bool IsValid(string role)
  {
    if (string.IsNullOrWhiteSpace(role)) return false;
    return role.Equals("Manager", StringComparison.OrdinalIgnoreCase) ||
           role.Equals("Editor", StringComparison.OrdinalIgnoreCase) ||
           role.Equals("Reader", StringComparison.OrdinalIgnoreCase);
  }
}
