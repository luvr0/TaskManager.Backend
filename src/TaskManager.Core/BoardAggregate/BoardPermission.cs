using Vogen;

namespace TaskManager.Core.BoardAggregate;

[ValueObject<string>(conversions: Conversions.SystemTextJson)]
public partial struct BoardPermission
{
  public const int MaxLength = 100;

  private static Validation Validate(in string permission) =>
    string.IsNullOrWhiteSpace(permission)
      ? Validation.Invalid("BoardPermission cannot be empty")
      : permission.Length > MaxLength
        ? Validation.Invalid($"BoardPermission cannot be longer than {MaxLength} characters")
        : Validation.Ok;

  public static BoardPermission Create => From("Create");
  public static BoardPermission Read => From("Read");
  public static BoardPermission Update => From("Update");
  public static BoardPermission Delete => From("Delete");

  public static IReadOnlyList<BoardPermission> GetAllBoardPermissions() => new List<BoardPermission>
  {
    Create, Read, Update, Delete
  };

  public static IReadOnlyList<BoardPermission> GetPermissionsForBoardRole(BoardRole role)
  {
    if (role == BoardRole.Manager)
    {
      return GetAllBoardPermissions();
    }
    else if (role == BoardRole.Editor)
    {
      return new List<BoardPermission>
      {
        Read,
        Update
      };
    }
    else if (role == BoardRole.Reader)
    {
      return new List<BoardPermission>
      {
        Read
      };
    }

    return new List<BoardPermission>();
  }
}
