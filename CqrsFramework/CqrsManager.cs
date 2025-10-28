using System.Diagnostics;
using CqrsFramework.Command;
using CqrsFramework.Event;
using CqrsFramework.Query;
using CqrsFramework.Validation;

namespace CqrsFramework;

public interface ICqrsManager
{
  /// <summary>
  /// Executes the specified command.
  /// </summary>
  /// <param name="command"></param>
  /// <param name="cancellationToken">Cancellation token.</param>
  Task ExecuteAsync(ICommand command, CancellationToken cancellationToken = default);
    
  /// <summary>
  /// Executes the specified query.
  /// </summary>
  /// <typeparam name="TResult">The type of the query result.</typeparam>
  /// <param name="query">The query to execute.</param>
  /// <param name="cancellationToken">Cancellation token.</param>
  /// <returns>The result of the query.</returns>
  Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);

  /// <summary>
  /// Publishes the specified event to all subscribers.
  /// </summary>
  /// <param name="event">The event to publish.</param>
  /// <param name="cancellationToken">Cancellation token.</param>
  Task PublishEventAsync(IEvent @event, CancellationToken cancellationToken = default);

  /// <summary>
  /// Validates the specified object using registered validators.
  /// </summary>
  /// <typeparam name="T">The object type.</typeparam>
  /// <param name="obj">The object to validate.</param>
  /// <param name="cancellationToken">Cancellation token</param>
  /// <returns>The validation result.</returns>
  Task<ValidationResult> ValidateAsync<T>(T obj, CancellationToken cancellationToken = default);
}

[DebuggerStepThrough]
public class CqrsManager(ICommandProcessor commandProcessor, IQueryProcessor queryProcessor, IEventProcessor eventProcessor, IValidationProcessor validationProcessor) : ICqrsManager
{
  private readonly ICommandProcessor _commandProcessor = commandProcessor ?? throw new ArgumentNullException(nameof(commandProcessor));
  private readonly IValidationProcessor _validationProcessor = validationProcessor ?? throw new ArgumentNullException(nameof(validationProcessor));
  private readonly IQueryProcessor _queryProcessor = queryProcessor ?? throw new ArgumentNullException(nameof(queryProcessor));
  private readonly IEventProcessor _eventProcessor = eventProcessor ?? throw new ArgumentNullException(nameof(eventProcessor));

  /// <inheritdoc />
  public async Task ExecuteAsync(ICommand command, CancellationToken cancellationToken = default) => await _commandProcessor.ProcessAsync(command, cancellationToken);
    
  /// <inheritdoc />
  public async Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default) => await _queryProcessor.ProcessAsync(query, cancellationToken);
    
  /// <inheritdoc />
  public async Task PublishEventAsync(IEvent @event, CancellationToken cancellationToken = default) => await _eventProcessor.ProcessAsync(@event, cancellationToken);
    
  /// <inheritdoc />
  public async Task<ValidationResult> ValidateAsync<T>(T obj, CancellationToken cancellationToken = default) => await _validationProcessor.ProcessValidationAsync(obj, cancellationToken);
}