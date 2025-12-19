using System.ComponentModel.DataAnnotations;

namespace TaskManager.Web.Boards;

public class UpdateBoardRequest
{
  public const string Route = "/Boards/{BoardId:int}";
  public static string BuildRoute(int boardId) => Route.Replace("{BoardId:int}", boardId.ToString());

  public int BoardId { get; set; }

  [Required]
  public int Id { get; set; }
  [Required]
  public string? Name { get; set; }
}
