namespace CQRSLib.Commands;

public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    Task ExecuteAsync(TCommand command);
}

public interface ICommandHandler<in TCommand, TCommandResult> where TCommand : ICommand<TCommandResult>
{
    Task<TCommandResult> ExecuteAsync(TCommand command);
}