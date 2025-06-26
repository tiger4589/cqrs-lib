using CQRSLib.Commands;
using CQRSLib.Queries;

namespace CQRSLib.Core;

public interface IDispatcher
{
    Task DispatchAsync<TCommand>(TCommand command) where TCommand : ICommand;
    Task<TCommandResult> DispatchAsync<TCommandResult>(ICommand<TCommandResult> command);
    Task<TQueryResult> DispatchAsync<TQueryResult>(IQuery<TQueryResult> query) where TQueryResult : IQueryResult;
}