using CQRSLib.Commands;
using CQRSLib.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace CQRSLib.Core;

internal sealed class Dispatcher(IServiceProvider serviceProvider) : IDispatcher
{
    public Task DispatchAsync<TCommand>(TCommand command) where TCommand : ICommand
    {
        var handler = GetHandler(command);
        return handler.ExecuteAsync(command);
    }

    public Task<TQueryResult> DispatchAsync<TQuery, TQueryResult>(TQuery query) where TQuery : IQuery<TQueryResult> where TQueryResult : IQueryResult
    {
        var handler = GetHandler<TQuery, TQueryResult>(query);
        return handler.RetrieveAsync(query);
    }

    private ICommandHandler<TCommand> GetHandler<TCommand>(TCommand command)
        where TCommand : ICommand
    {
        var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();
        return handler;
    }

    private IQueryHandler<TQuery, TQueryResult> GetHandler<TQuery, TQueryResult>(TQuery query)
        where TQuery : IQuery<TQueryResult> where TQueryResult : IQueryResult
    {
        var handler = serviceProvider.GetRequiredService<IQueryHandler<TQuery, TQueryResult>>();
        return handler;
    }
}