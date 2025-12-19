using System;
using System.Net;
using System.Net.Http.Json;
using TaskManager.Web.Auth;

namespace TaskManager.FunctionalTests.ApiEndpoints;

public class AuthenticationEndpointsTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client;

  public AuthenticationEndpointsTests(CustomWebApplicationFactory<Program> factory)
  {
    _client = factory.CreateClient();
  }

  [Fact]
  public async Task RegisterAndLoginFlow_ReturnsJwtAndRefreshCookie()
  {
    var email = $"candidate+{Guid.NewGuid():N}@gmail.com";
    var password = "S3cur3P@ss!";

    var registerResponse = await _client.PostAsJsonAsync(RegisterRequest.Route, new RegisterRequest
    {
      Name = "Test Candidate",
      Email = email,
      Password = password
    });

    registerResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
    var registeredUser = await registerResponse.Content.ReadFromJsonAsync<RegisterResponse>();
    registeredUser.ShouldNotBeNull();
    registeredUser.Email.ShouldBe(email);

    var loginResponse = await _client.PostAsJsonAsync(LoginRequest.Route, new LoginRequest
    {
      Email = email,
      Password = password
    });

    loginResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
    var auth = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
    auth.ShouldNotBeNull();
    auth.AccessToken.ShouldNotBeNullOrWhiteSpace();

    loginResponse.Headers.TryGetValues("Set-Cookie", out var cookies).ShouldBeTrue();
    cookies.ShouldContain(value => value.StartsWith("refreshToken=", StringComparison.OrdinalIgnoreCase));
  }
}
