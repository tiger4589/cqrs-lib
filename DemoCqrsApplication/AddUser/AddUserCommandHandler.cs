using CQRSLib.Commands;

namespace DemoCqrsApplication.AddUser;

public class AddUserCommandHandler : ICommandHandler<AddUserCommand, Guid>
{
    public Task<Guid> ExecuteAsync(AddUserCommand command)
    {
        var guid = Guid.NewGuid();
        Console.WriteLine($"Added User {guid}");
        return Task.FromResult(guid);
    }
}