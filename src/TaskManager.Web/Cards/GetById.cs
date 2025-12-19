using TaskManager.Core.BoardAggregate;
using TaskManager.UseCases.Boards;
using TaskManager.UseCases.Cards.Get;
using TaskManager.Web.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TaskManager.Web.Cards;

public class GetById(IMediator mediator)
  : Endpoint<GetCardByIdRequest,
             Results<Ok<CardDetailResponse>, NotFound, ProblemHttpResult>,
             GetCardByIdMapper>
{
  public override void Configure()
  {
    Get(GetCardByIdRequest.Route);
    Policies("RequireAuthenticatedUser");

    Summary(s =>
    {
      s.Summary = "Get a card by ID";
      s.Description = "Retrieves a specific card by its unique identifier.";
      s.ExampleRequest = new GetCardByIdRequest { CardId = 1, BoardId = 1 };
      s.ResponseExamples[200] = new CardDetailResponse(1, "Implement login", "Add user authentication", 1, "Pending");

      s.Responses[200] = "Card found and returned successfully";
      s.Responses[401] = "Authentication required";
      s.Responses[404] = "Card or Board not found";
    });

    Tags("Cards");

    Description(builder => builder
      .Accepts<GetCardByIdRequest>()
      .Produces<CardDetailResponse>(200, "application/json")
      .ProducesProblem(401)
      .ProducesProblem(404));
  }

  public override async Task<Results<Ok<CardDetailResponse>, NotFound, ProblemHttpResult>>
    ExecuteAsync(GetCardByIdRequest request, CancellationToken ct)
  {
    var result = await mediator.Send(
      new GetCardQuery(
        CardId.From(request.CardId),
        BoardId.From(request.BoardId)),
      ct);

    return result.ToGetByIdResult(Map.FromEntity);
  }
}

public sealed class GetCardByIdMapper
  : Mapper<GetCardByIdRequest, CardDetailResponse, CardDto>
{
  public override CardDetailResponse FromEntity(CardDto e)
    => new(
      e.Id.Value,
      e.Title.Value,
      e.Description?.Value ?? string.Empty,
      e.Order.Value,
      e.Status.ToString());
}
