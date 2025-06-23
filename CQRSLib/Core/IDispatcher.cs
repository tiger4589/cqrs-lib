using CQRSLib.Commands;
using CQRSLib.Queries;

namespace CQRSLib.Core;

public interface IDispatcher
{
    Task DispatchAsync<TCommand>(TCommand command) where TCommand : ICommand;
    Task<TCommandResult> DispatchAsync<TCommand, TCommandResult>(TCommand command) where TCommand : ICommand<TCommandResult>;
    Task<TQueryResult> RetrieveAsync<TQuery, TQueryResult>(TQuery query) where TQuery : IQuery<TQueryResult> where TQueryResult : IQueryResult;
}