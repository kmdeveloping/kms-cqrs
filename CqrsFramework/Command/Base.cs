using System.Diagnostics;

namespace CqrsFramework.Command;

[DebuggerStepThrough]
public abstract class Base : ICommand
{
  /// <inheritdoc />
  public bool ExecuteAsNoOp { get; set; }
}