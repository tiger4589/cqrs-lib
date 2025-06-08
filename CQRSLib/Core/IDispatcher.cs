using CQRSLib.Commands;
using CQRSLib.Queries;

namespace CQRSLib.Core;

public interface IDispatcher
{
    Task DispatchAsync<TCommand>(TCommand command) where TCommand : ICommand;
    Task<TQueryResult> DispatchAsync<TQuery, TQueryResult>(TQuery query) where TQuery : IQuery where TQueryResult : IQueryResult;
}