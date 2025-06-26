using CQRSLib.Commands;
using CQRSLib.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace CQRSLib.Core;

internal sealed class Dispatcher(IServiceProvider serviceProvider) : IDispatcher
{
    public Task DispatchAsync<TCommand>(TCommand command) where TCommand : ICommand
    {
        var handler = GetHandler<TCommand>();
        return handler.ExecuteAsync(command);
    }

    public Task<TCommandResult> DispatchAsync<TCommandResult>(ICommand<TCommandResult> command)
    {
        var handler = GetHandler(command);
        return handler.ExecuteAsync((dynamic)command);
    }

    public Task<TQueryResult> DispatchAsync<TQueryResult>(IQuery<TQueryResult> query) where TQueryResult : IQueryResult
    {
        var handler = GetQueryHandler(query);
        return handler.RetrieveAsync((dynamic)query);
    }

    private ICommandHandler<TCommand> GetHandler<TCommand>()
        where TCommand : ICommand
    {
        var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();
        return handler;
    }

    private dynamic GetHandler<TCommandResult>(ICommand<TCommandResult> command)
    {
        var commandType = command.GetType();
        var handlerType = typeof(ICommandHandler<,>)
            .MakeGenericType(commandType,
                typeof(TCommandResult));

        dynamic handler = serviceProvider.GetRequiredService(handlerType);
        return handler;
    }

    private dynamic GetQueryHandler<TQueryResult>(IQuery<TQueryResult> query) where TQueryResult : IQueryResult
    {
        var queryType = query.GetType();
        var handlerType = typeof(IQueryHandler<,>)
            .MakeGenericType(queryType,
                typeof(TQueryResult));

        dynamic handler = serviceProvider.GetRequiredService(handlerType);
        return handler;
    }
}