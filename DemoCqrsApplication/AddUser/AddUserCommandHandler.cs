using CQRSLib.Commands;

namespace DemoCqrsApplication.AddUser;

public class AddUserCommandHandler : ICommandHandler<AddUserCommand>
{
    public Task ExecuteAsync(AddUserCommand command)
    {
        Console.WriteLine("Added User");
        return Task.CompletedTask;
    }
}