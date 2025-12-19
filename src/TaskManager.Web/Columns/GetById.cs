using TaskManager.Core.BoardAggregate;
using TaskManager.UseCases.Boards;
using TaskManager.UseCases.Columns.Get;
using TaskManager.Web.Extensions;
using TaskManager.Web.Boards;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TaskManager.Web.Columns;

public class GetById(IMediator mediator)
  : Endpoint<GetColumnByIdRequest,
             Results<Ok<ColumnResponse>,
                     NotFound,
                     ProblemHttpResult>,
             GetColumnByIdMapper>
{
  public override void Configure()
  {
    Get(GetColumnByIdRequest.Route);
    Policies("RequireAuthenticatedUser");

    Summary(s =>
    {
      s.Summary = "Get a column by ID";
      s.Description = "Retrieves a specific column by its unique identifier including its cards.";
      s.ExampleRequest = new GetColumnByIdRequest { ColumnId = 1, BoardId = 1 };
      s.ResponseExamples[200] = new ColumnResponse(1, "To Do", 1, new List<CardResponse>());

      s.Responses[200] = "Column found and returned successfully";
      s.Responses[401] = "Authentication required";
      s.Responses[404] = "Column or Board not found";
    });

    Tags("Columns");

    Description(builder => builder
      .Accepts<GetColumnByIdRequest>()
      .Produces<ColumnResponse>(200, "application/json")
      .ProducesProblem(401)
      .ProducesProblem(404));
  }

  public override async Task<Results<Ok<ColumnResponse>, NotFound, ProblemHttpResult>>
    ExecuteAsync(GetColumnByIdRequest request, CancellationToken ct)
  {
    var result = await mediator.Send(
      new GetColumnQuery(
        ColumnId.From(request.ColumnId), 
        BoardId.From(request.BoardId)), 
      ct);

    return result.ToGetByIdResult(Map.FromEntity);
  }
}

public sealed class GetColumnByIdMapper
  : Mapper<GetColumnByIdRequest, ColumnResponse, ColumnDto>
{
  public override ColumnResponse FromEntity(ColumnDto e)
  {
    var cards = e.Cards.Select(c => new CardResponse(
      c.Id.Value,
      c.Title.Value,
      c.Description?.Value ?? string.Empty,
      c.Order.Value,
      c.Status.ToString())).ToList();

    return new ColumnResponse(e.Id.Value, e.Name.Value, e.Order.Value, cards);
  }
}
