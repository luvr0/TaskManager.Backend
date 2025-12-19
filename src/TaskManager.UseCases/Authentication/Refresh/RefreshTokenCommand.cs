namespace TaskManager.UseCases.Authentication.Refresh;

public sealed record RefreshTokenCommand(string RefreshToken)
  : ICommand<Result<AuthTokensDto>>;
