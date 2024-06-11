using CqrsFramework.Command;

namespace CqrsFramework.Events;

public class OnBeforeCommandHandled<TCommand> : EventBase where TCommand : ICommand
{
  public OnBeforeCommandHandled(TCommand command)
  {
    if (command == null) throw new ArgumentNullException(nameof(command));
            
    Command = command;
  }
        
  public TCommand Command { get; private set; }
}