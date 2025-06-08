﻿namespace CQRSLib.Commands;

public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    Task ExecuteAsync(TCommand command);
}