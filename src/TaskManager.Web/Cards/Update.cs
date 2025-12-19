using TaskManager.Core.BoardAggregate;
using TaskManager.UseCases.Boards;
using TaskManager.UseCases.Cards.Update;
using TaskManager.Web.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TaskManager.Web.Cards;

public class Update(IMediator mediator)
  : Endpoint<UpdateCardRequest,
             Results<Ok<UpdateCardResponse>, NotFound, ProblemHttpResult>,
             UpdateCardMapper>
{
  public override void Configure()
  {
    Put(UpdateCardRequest.Route);
    Policies("RequireAuthenticatedUser");

    Summary(s =>
    {
      s.Summary = "Update a card";
      s.Description = "Updates an existing card's information. You can update title, description and/or status.";
      s.ExampleRequest = new UpdateCardRequest { CardId = 1, BoardId = 1, Title = "Updated task", Description = "New description", Status = "Done" };
      s.ResponseExamples[200] = new UpdateCardResponse(new CardRecord(1, "Updated task", "New description", 1, 1, "Done"));

      s.Responses[200] = "Card updated successfully";
      s.Responses[401] = "Authentication required";
      s.Responses[404] = "Card or Board not found";
      s.Responses[400] = "Invalid input data or business rule violation";
    });

    Tags("Cards");

    Description(builder => builder
      .Accepts<UpdateCardRequest>("application/json")
      .Produces<UpdateCardResponse>(200, "application/json")
      .ProducesProblem(401)
      .ProducesProblem(404)
      .ProducesProblem(400));
  }

  public override async Task<Results<Ok<UpdateCardResponse>, NotFound, ProblemHttpResult>>
    ExecuteAsync(UpdateCardRequest request, CancellationToken ct)
  {
    if (!HttpContext.TryGetUserId(out var userId))
    {
      return TypedResults.Problem(title: "Unauthorized", statusCode: StatusCodes.Status401Unauthorized);
    }

    CardTitle? title = string.IsNullOrEmpty(request.Title) 
      ? null 
      : CardTitle.From(request.Title);

    CardDescription? description = string.IsNullOrEmpty(request.Description) 
      ? null 
      : CardDescription.From(request.Description);

    CardStatus? status = string.IsNullOrEmpty(request.Status)
      ? null
      : CardStatus.FromName(request.Status, ignoreCase: true);

    var cmd = new UpdateCardCommand(
      CardId.From(request.CardId),
      BoardId.From(request.BoardId),
      title,
      description,
      status,
      userId);

    var result = await mediator.Send(cmd, ct);

    return result.ToUpdateResult(Map.FromEntity);
  }
}

public sealed class UpdateCardMapper
  : Mapper<UpdateCardRequest, UpdateCardResponse, CardDto>
{
  public override UpdateCardResponse FromEntity(CardDto e)
    => new(new CardRecord(e.Id.Value, e.Title.Value, e.Description?.Value ?? string.Empty, e.ColumnId.Value, e.Order.Value, e.Status.ToString()));
}
