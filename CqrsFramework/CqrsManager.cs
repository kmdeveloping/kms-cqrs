using CqrsFramework.Command;
using CqrsFramework.Events;
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
  /// <returns></returns>
  Task<ValidationResult> ValidateAsync<T>(T obj, CancellationToken cancellationToken = default);
}

public class CqrsManager : ICqrsManager
{
  private readonly ICommandProcessor _commandProcessor;
  private readonly IValidationProcessor _validationProcessor;
  private readonly IQueryProcessor _queryProcessor;
  private readonly IEventProcessor _eventProcessor;
  
  public CqrsManager(ICommandProcessor commandProcessor,
    IValidationProcessor validationProcessor,
    IQueryProcessor queryProcessor, 
    IEventProcessor eventProcessor)
  {
    _commandProcessor = commandProcessor ?? throw new ArgumentNullException(nameof(commandProcessor));
    _validationProcessor = validationProcessor ?? throw new ArgumentNullException(nameof(validationProcessor));
    _queryProcessor = queryProcessor ?? throw new ArgumentNullException(nameof(queryProcessor));
    _eventProcessor = eventProcessor ?? throw new ArgumentNullException(nameof(eventProcessor));
  }
  
  /// <inheritdoc />
  public async Task ExecuteAsync(ICommand command, CancellationToken cancellationToken = default)
  {
    await _commandProcessor.ProcessAsync(command, cancellationToken);
  }

  /// <inheritdoc />
  public async Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
  {
    return await _queryProcessor.ProcessAsync(query, cancellationToken);
  }

  /// <inheritdoc />
  public async Task PublishEventAsync(IEvent @event, CancellationToken cancellationToken = default)
  {
    await _eventProcessor.ProcessAsync(@event, cancellationToken);
  }
  
  /// <inheritdoc />
  public async Task<ValidationResult> ValidateAsync<T>(T obj, CancellationToken cancellationToken = default)
  {
    return await _validationProcessor.ProcessValidationAsync(obj, cancellationToken);
  }
}