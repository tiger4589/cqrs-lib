using CQRSLib.Commands;

namespace DemoCqrsApplication.AddUser;

public sealed record AddUserCommand : ICommand<Guid>;