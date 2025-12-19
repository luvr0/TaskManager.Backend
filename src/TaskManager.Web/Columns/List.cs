using Microsoft.AspNetCore.Http.HttpResults;
using TaskManager.Core.BoardAggregate;
using TaskManager.UseCases.Boards;
using TaskManager.UseCases.Columns.List;
using TaskManager.Web.Extensions;

namespace TaskManager.Web.Columns;

public class List(IMediator mediator)
  : Endpoint<ListColumnsRequest, Results<Ok<ColumnListResponse>, ProblemHttpResult>>
{
  public override void Configure()
  {
    Get(ListColumnsRequest.Route);
    Policies("RequireAuthenticatedUser");

    Summary(s =>
    {
      s.Summary = "List columns for a board";
      s.Description = "Retrieves a paginated list of columns that belong to the specified board.";
      s.Params[nameof(ListColumnsRequest.BoardId)] = "Board identifier (route parameter)";
      s.Params["page"] = "1-based page index (default 1)";
      s.Params["per_page"] = $"Page size 1–{UseCases.Constants.MAX_PAGE_SIZE} (default {UseCases.Constants.DEFAULT_PAGE_SIZE})";
      s.ExampleRequest = new ListColumnsRequest { BoardId = 1, Page = 1, PerPage = 10 };

      s.Responses[200] = "Columns retrieved successfully";
      s.Responses[401] = "Authentication required";
      s.Responses[400] = "Invalid pagination parameters";
    });

    Tags("Columns");
  }

  public override async Task<Results<Ok<ColumnListResponse>, ProblemHttpResult>>
    ExecuteAsync(ListColumnsRequest request, CancellationToken ct)
  {
    var result = await mediator.Send(
      new ListColumnQuery(BoardId.From(request.BoardId), request.Page, request.PerPage),
      ct);

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

  private static ColumnListResponse MapToResponse(UseCases.PagedResult<ColumnDto> e)
  {
    var items = e.Items
      .Select(c => new ColumnRecord(c.Id.Value, c.Name.Value, c.BoardId.Value, c.Order.Value))
      .ToList();

    return new ColumnListResponse(items, e.Page, e.PerPage, e.TotalCount, e.TotalPages);
  }
}
