using CQRSLib.Commands;

namespace DemoCqrsApplication.DeleteUser;

public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
{
    public Task ExecuteAsync(DeleteUserCommand command)
    {
        Console.WriteLine("Deleted User");
        return Task.CompletedTask;
    }
}