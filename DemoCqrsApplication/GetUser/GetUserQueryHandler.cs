using CQRSLib.Queries;

namespace DemoCqrsApplication.GetUser;

public class GetUserQueryHandler : IQueryHandler<GetUserQuery, GetUserQueryResult>
{
    public Task<GetUserQueryResult> RetrieveAsync(GetUserQuery query)
    {
        Console.WriteLine("Here's your User");
        return Task.FromResult(new GetUserQueryResult());
    }
}