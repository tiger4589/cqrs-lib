using CQRSLib.Queries;

namespace DemoCqrsApplication.GetUser;

public sealed record GetUserQuery(Guid Id) : IQuery<GetUserQueryResult>;