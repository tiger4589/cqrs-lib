using CQRSLib.Commands;
using CQRSLib.Queries;

namespace DemoCqrsApplication;

public class AddUserCommand : ICommand
{
    
}

public class AddUserCommandHandler : ICommandHandler<AddUserCommand>
{
    public Task ExecuteAsync(AddUserCommand command)
    {
        Console.WriteLine("Added User");
        return Task.CompletedTask;
    }
}

public class DeleteUserCommand : ICommand;

public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
{
    public Task ExecuteAsync(DeleteUserCommand command)
    {
        Console.WriteLine("Deleted User");
        return Task.CompletedTask;
    }
}

public class GetUserQuery : IQuery;

public class GetUserQueryResult : IQueryResult;

public class GetUserQueryHandler : IQueryHandler<GetUserQuery, GetUserQueryResult>
{
    public Task<GetUserQueryResult> RetrieveAsync(GetUserQuery query)
    {
        Console.WriteLine("Here's your User");
        return Task.FromResult(new GetUserQueryResult());
    }
}

public class GetUsersQuery : IQuery;

public class GetUsersQueryResult : IQueryResult;

public class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, GetUsersQueryResult>
{
    public Task<GetUsersQueryResult> RetrieveAsync(GetUsersQuery query)
    {
        Console.WriteLine("Here's your Users");
        return Task.FromResult(new GetUsersQueryResult());
    }
}