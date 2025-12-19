using Microsoft.AspNetCore.Http.HttpResults;
using TaskManager.UseCases.Boards;
using TaskManager.UseCases.Boards.List;
using TaskManager.Web.Extensions;

namespace TaskManager.Web.Boards;

public class List(IMediator mediator) 
  : Endpoint<ListBoardsRequest, Results<Ok<BoardListResponse>, UnauthorizedHttpResult, ProblemHttpResult>>
{
  public override void Configure()
  {
    Get(ListBoardsRequest.Route);
    Summary(s =>
    {
      s.Summary = "List boards with pagination";
      s.Description = "Retrieves a paginated list of boards that belong to the authenticated user either as owner or member.";
      s.ExampleRequest = new ListBoardsRequest { Page = 1, PerPage = 10 };
      s.ResponseExamples[200] = new BoardListResponse(
        new List<BoardRecord>
        {
          new(1, "Project Alpha", 1, 2,
            new List<MemberSummaryResponse>
            {
              new(1, "Manager", "manager@gmail.com", "MG"),
              new(2, "Editor", "editor@gmail.com", "EU")
            }),
          new(2, "Project Beta", 1, 2,
            new List<MemberSummaryResponse>
            {
              new(1, "Manager", "manager@gmail.com", "MG"),
              new(2, "Editor", "editor@gmail.com", "EU")
            })
        },
        1, 10, 2, 1);

      s.Params["page"] = "1-based page index (default 1)";
      s.Params["per_page"] = $"Page size 1–{UseCases.Constants.MAX_PAGE_SIZE} (default {UseCases.Constants.DEFAULT_PAGE_SIZE})";

      s.Responses[200] = "Paginated list of boards returned successfully";
      s.Responses[400] = "Invalid pagination parameters";
      s.Responses[401] = "Authentication required";
    });

    Tags("Boards");

    Description(builder => builder
      .Accepts<ListBoardsRequest>()
      .Produces<BoardListResponse>(200, "application/json")
      .ProducesProblem(400)
      .ProducesProblem(401));
  }

  public override async Task<Results<Ok<BoardListResponse>, UnauthorizedHttpResult, ProblemHttpResult>>
    ExecuteAsync(ListBoardsRequest request, CancellationToken ct)
  {
    if (!HttpContext.TryGetUserId(out var userId))
    {
      return TypedResults.Unauthorized();
    }

    var result = await mediator.Send(new ListBoardQuery(userId, request.Page, request.PerPage), ct);
    if (!result.IsSuccess)
    {
      return TypedResults.Problem(
        title: "List failed",
        detail: string.Join("; ", result.Errors),
        statusCode: StatusCodes.Status400BadRequest);
    }

    var pagedResult = result.Value;
    HttpContext.AddPaginationLinkHeader(pagedResult.Page, pagedResult.PerPage, pagedResult.TotalPages);

    var response = MapToResponse(pagedResult);
    return TypedResults.Ok(response);
  }

  private static BoardListResponse MapToResponse(UseCases.PagedResult<BoardDto> e)
  {
    var items = e.Items
      .Select(b => new BoardRecord(
        b.Id.Value,
        b.Name.Value,
        b.OwnerId.Value,
        b.Members.Count,
        b.Members
          .Select(m => new MemberSummaryResponse(m.Id.Value, m.Role.Value, m.Email, m.EmailAlias))
          .ToList()))
      .ToList();

    return new BoardListResponse(items, e.Page, e.PerPage, e.TotalCount, e.TotalPages);
  }
}
