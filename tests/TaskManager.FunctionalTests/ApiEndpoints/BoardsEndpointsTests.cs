using System;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TaskManager.Web.Auth;
using TaskManager.Web.Boards;

namespace TaskManager.FunctionalTests.ApiEndpoints;

public class BoardsEndpointsTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client;

  public BoardsEndpointsTests(CustomWebApplicationFactory<Program> factory)
  {
    _client = factory.CreateClient();
  }

  [Fact]
  public async Task CreateAndListBoards_PersistsDataForAuthenticatedUser()
  {
    var user = await RegisterAndLoginAsync();
    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.AccessToken);

    var boardName = $"Discovery-{Guid.NewGuid():N}";
    var createResponse = await _client.PostAsJsonAsync(CreateBoardRequest.Route, new CreateBoardRequest
    {
      Name = boardName
    });

    createResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
    var board = await createResponse.Content.ReadFromJsonAsync<CreateBoardResponse>();
    board.ShouldNotBeNull();
    board.Name.ShouldBe(boardName);

    var listResponse = await _client.GetAsync("/Boards?page=1&per_page=5");
    listResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
    var list = await listResponse.Content.ReadFromJsonAsync<BoardListResponse>();
    list.ShouldNotBeNull();
    list.Items.ShouldContain(b => b.Name == boardName);
    list.TotalCount.ShouldBeGreaterThanOrEqualTo(1);
  }

  private async Task<AuthResponse> RegisterAndLoginAsync()
  {
    var email = $"owner+{Guid.NewGuid():N}@gmail.com";
    const string password = "S3cur3P@ss!";

    var registerRequest = new RegisterRequest
    {
      Name = "Board Owner",
      Email = email,
      Password = password
    };

    var registerResponse = await _client.PostAsJsonAsync(RegisterRequest.Route, registerRequest);
    registerResponse.EnsureSuccessStatusCode();

    var loginResponse = await _client.PostAsJsonAsync(LoginRequest.Route, new LoginRequest
    {
      Email = email,
      Password = password
    });

    loginResponse.EnsureSuccessStatusCode();
    var auth = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
    return auth!;
  }
}
