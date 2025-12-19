using Ardalis.Result;
using Ardalis.Specification;
using System.Threading;
using System.Reflection;
using TaskManager.Core.Interfaces;
using TaskManager.Core.UserAggregate;
using TaskManager.UseCases.Authentication;
using TaskManager.UseCases.Authentication.Login;

namespace TaskManager.UnitTests.UseCases.Authentication;

public class AuthenticateUserHandlerTests
{
  private readonly IRepository<User> _repository = Substitute.For<IRepository<User>>();
  private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
  private readonly ITokenService _token_service = Substitute.For<ITokenService>();
  private readonly ILogger<AuthenticateUserHandler> _logger = Substitute.For<ILogger<AuthenticateUserHandler>>();

  [Fact]
  public async Task Handle_ReturnsUnauthorized_WhenUserNotFound()
  {
    var handler = CreateHandler();
    var command = new AuthenticateUserCommand(UserEmail.From("missing@gmail.com"), "Password123!");

    var result = await handler.Handle(command, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.Unauthorized);
  }

  [Fact]
  public async Task Handle_ReturnsUnauthorized_WhenPasswordIsInvalid()
  {
    var user = CreateUser("user@gmail.com");
    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<User>>(), Arg.Any<CancellationToken>()).Returns(user);
    _passwordHasher.VerifyPassword("Password123!", user.PasswordHash.Value).Returns(false);

    var handler = CreateHandler();
    var command = new AuthenticateUserCommand(user.Email, "Password123!");

    var result = await handler.Handle(command, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.Unauthorized);
    await _repository.DidNotReceive().UpdateAsync(user, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_ReturnsTokensAndPersistsRefreshToken_WhenCredentialsAreValid()
  {
    var user = CreateUser("user@gmail.com");
    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<User>>(), Arg.Any<CancellationToken>()).Returns(user);
    _passwordHasher.VerifyPassword("Password123!", user.PasswordHash.Value).Returns(true);
    _token_service.GenerateAccessToken(user).Returns("access-token");
    var refreshToken = RefreshToken.From("refresh-token");
    _token_service.GenerateRefreshToken().Returns(refreshToken);

    var handler = CreateHandler();
    var command = new AuthenticateUserCommand(user.Email, "Password123!");

    var result = await handler.Handle(command, CancellationToken.None);

    result.Status.ShouldBe(ResultStatus.Ok);
    result.Value.AccessToken.ShouldBe("access-token");
    user.RefreshToken.ShouldBe(refreshToken);
    user.RefreshTokenExpiryTime.ShouldNotBeNull();
    await _repository.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
  }

  private AuthenticateUserHandler CreateHandler() => new(_repository, _passwordHasher, _token_service, _logger);

  private static User CreateUser(string email)
  {
    var user = new User(
      UserName.From("Test User"),
      UserEmail.From(email),
      UserPassword.From("hashed"));

    // set Id via reflection
    var idProp = typeof(User).GetProperty("Id");
    if (idProp != null && idProp.CanWrite)
    {
      idProp.SetValue(user, UserId.From(1));
    }
    else
    {
      var field = typeof(User).GetField("<Id>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
      if (field != null) field.SetValue(user, UserId.From(1));
    }

    return user;
  }
}
