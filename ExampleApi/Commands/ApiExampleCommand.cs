using System.ComponentModel.DataAnnotations;
using cqrsCore.Command;

namespace ExampleApi.Commands;

public class ApiExampleCommand : ICommandWithResult<string>
{
  [Required]
  public string inputName { get; set; }
  [Required]
  public DateTime? TimeStamp { get; set; }

  public string Result { get; set; }
  public bool ExecuteAsNoOp { get; set; } = false;
  /// <summary>
  /// Optional contextual data which can be attached to commands.
  /// </summary>
  public IDictionary<string, object> ContextData { get; set; }
}