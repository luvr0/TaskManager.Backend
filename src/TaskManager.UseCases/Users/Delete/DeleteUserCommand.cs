namespace TaskManager.UseCases.Users.Delete;

public record DeleteUserCommand(int UserId) : ICommand<Result>;
