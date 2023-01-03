
using cqrsCore.Command;

namespace cqrsCore.Events;

public class OnAfterCommandHandled<TCommand> : EventBase where TCommand : ICommand
{
  public OnAfterCommandHandled(TCommand command)
  {
    if (command == null) throw new ArgumentNullException(nameof(command));
            
    Command = command;
  }

  public TCommand Command { get; private set; }
}