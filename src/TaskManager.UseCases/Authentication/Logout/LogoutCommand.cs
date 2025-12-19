namespace TaskManager.UseCases.Authentication.Logout;

public sealed record LogoutCommand(string RefreshToken) : ICommand<Result>;
