
using System.ComponentModel.DataAnnotations;
using cqrsCore.Command;

namespace Examples.CommandContracts;

public class ExampleCommand : ICommand
{
  [Required]
  public string ExampleName { get; set; }
  
  [Required]
  public DateTime? TimeStamp { get; set; }
  
  public bool ExecuteAsNoOp { get; set; } = false;

  /// <summary>
  /// Optional contextual data which can be attached to commands.
  /// </summary>
  public IDictionary<string, object> ContextData { get; set; } = new Dictionary<string, object>();
}