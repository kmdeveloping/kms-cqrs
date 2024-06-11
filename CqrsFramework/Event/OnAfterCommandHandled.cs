using System.Diagnostics;
using CqrsFramework.Command;

namespace CqrsFramework.Event;

[DebuggerStepThrough]
public class OnAfterCommandHandled<TCommand> : EventBase
    where TCommand: ICommand
{
    public OnAfterCommandHandled(TCommand command)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));
            
        Command = command;
    }

    public TCommand Command { get; private set; }
}