using Microsoft.AspNetCore.Http.HttpResults;
using TaskManager.Core.BoardAggregate;
using TaskManager.UseCases.Boards;
using TaskManager.UseCases.Cards.List;
using TaskManager.Web.Extensions;

namespace TaskManager.Web.Cards;

public class List(IMediator mediator)
  : Endpoint<ListCardsRequest, Results<Ok<CardListResponse>, ProblemHttpResult>>
{
  public override void Configure()
  {
    Get(ListCardsRequest.Route);
    Policies("RequireAuthenticatedUser");

    Summary(s =>
    {
      s.Summary = "List cards for a column";
      s.Description = "Retrieves a paginated list of cards belonging to the specified column.";
      s.Params[nameof(ListCardsRequest.BoardId)] = "Board identifier (route parameter)";
      s.Params[nameof(ListCardsRequest.ColumnId)] = "Column identifier (route parameter)";
      s.Params["page"] = "1-based page index (default 1)";
      s.Params["per_page"] = $"Page size 1–{UseCases.Constants.MAX_PAGE_SIZE} (default {UseCases.Constants.DEFAULT_PAGE_SIZE})";
      s.ExampleRequest = new ListCardsRequest { BoardId = 1, ColumnId = 1, Page = 1, PerPage = 10 };

      s.Responses[200] = "Cards retrieved successfully";
      s.Responses[401] = "Authentication required";
      s.Responses[400] = "Invalid pagination parameters";
    });

    Tags("Cards");
  }

  public override async Task<Results<Ok<CardListResponse>, ProblemHttpResult>>
    ExecuteAsync(ListCardsRequest request, CancellationToken ct)
  {
    var result = await mediator.Send(
      new ListCardQuery(
        BoardId.From(request.BoardId),
        ColumnId.From(request.ColumnId),
        request.Page,
        request.PerPage),
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

  private static CardListResponse MapToResponse(UseCases.PagedResult<CardDto> e)
  {
    var items = e.Items
      .Select(card => new CardRecord(
        card.Id.Value,
        card.Title.Value,
        card.Description?.Value ?? string.Empty,
        card.ColumnId.Value,
        card.Order.Value,
        card.Status.ToString()))
      .ToList();

    return new CardListResponse(items, e.Page, e.PerPage, e.TotalCount, e.TotalPages);
  }
}
