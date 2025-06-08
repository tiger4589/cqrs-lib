namespace CQRSLib.Queries;

public interface IQueryHandler<in TQuery, TQueryResult> where TQuery : IQuery<TQueryResult> where TQueryResult : IQueryResult
{
    Task<TQueryResult> RetrieveAsync(TQuery  query);
}