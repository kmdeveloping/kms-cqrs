using cqrsCore.Command;
using cqrsCore.Validation;

namespace cqrsCore;

public interface ICqrsManager
{
  /// <summary>
  /// Executes the specified command.
  /// </summary>
  /// <param name="command"></param>
  /// <param name="cancellationToken">Cancellation token.</param>
  Task ExecuteAsync(ICommand command, CancellationToken cancellationToken = default);
  
  /// <summary>
  /// Validates the specified object using registered validators.
  /// </summary>
  /// <typeparam name="T">The object type.</typeparam>
  /// <param name="obj">The object to validate.</param>
  /// <param name="cancellationToken">Cancellation token</param>
  /// <returns></returns>
  Task<ValidationResult> ValidateAsync<T>(T obj, CancellationToken cancellationToken = default);
}

public class CqrsManager : ICqrsManager
{
  private readonly ICommandProcessor _commandProcessor;
  private readonly IValidationProcessor _validationProcessor;
  
  public CqrsManager(ICommandProcessor commandProcessor, IValidationProcessor validationProcessor)
  {
    _commandProcessor = commandProcessor ?? throw new ArgumentNullException(nameof(commandProcessor));
    _validationProcessor = validationProcessor ?? throw new ArgumentNullException(nameof(validationProcessor));
  }
  
  /// <inheritdoc />
  public async Task ExecuteAsync(ICommand command, CancellationToken cancellationToken = default)
  {
    await _commandProcessor.ProcessAsync(command, cancellationToken);
  }

  /// <inheritdoc />
  public async Task<ValidationResult> ValidateAsync<T>(T obj, CancellationToken cancellationToken = default)
  {
    return await _validationProcessor.ProcessValidationAsync(obj, cancellationToken);
  }
}