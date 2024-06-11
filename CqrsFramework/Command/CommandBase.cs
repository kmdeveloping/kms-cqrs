using System.Diagnostics;

namespace CqrsFramework.Command;

[DebuggerStepThrough]
public abstract class CommandBase : ICommand
{
    /// <inheritdoc />
    public bool ExecuteAsNoOp { get; set; }
}