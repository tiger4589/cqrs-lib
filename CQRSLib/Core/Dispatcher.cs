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

    public Task<TCommandResult> DispatchAsync<TCommand, TCommandResult>(TCommand command) where TCommand : ICommand<TCommandResult>
    {
        var handler = GetHandler<TCommand, TCommandResult>();
        return handler.ExecuteAsync(command);
    }

    public Task<TQueryResult> RetrieveAsync<TQuery, TQueryResult>(TQuery query) where TQuery : IQuery<TQueryResult> where TQueryResult : IQueryResult
    {
        var handler = GetQueryHandler<TQuery, TQueryResult>();
        return handler.RetrieveAsync(query);
    }

    private ICommandHandler<TCommand> GetHandler<TCommand>()
        where TCommand : ICommand
    {
        var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();
        return handler;
    }

    private ICommandHandler<TCommand, TCommandResult> GetHandler<TCommand, TCommandResult>()
        where TCommand : ICommand<TCommandResult>
    {
        var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand, TCommandResult>>();
        return handler;
    }

    private IQueryHandler<TQuery, TQueryResult> GetQueryHandler<TQuery, TQueryResult>()
        where TQuery : IQuery<TQueryResult> where TQueryResult : IQueryResult
    {
        var handler = serviceProvider.GetRequiredService<IQueryHandler<TQuery, TQueryResult>>();
        return handler;
    }
}