using System.Diagnostics;
using CqrsFramework.Logging;
using CqrsFramework.Validation;

namespace CqrsFramework.Decorators.Validation;

public class LoggingValidatorDecorator<T> : IValidator<T> where T : class
{
  private readonly IValidator<T> _decoratedValidator;
  private readonly ILogger _logger;
  
  public LoggingValidatorDecorator(IValidator<T> decoratedValidator, ILogger logger)
  {
    _decoratedValidator = decoratedValidator ?? throw new ArgumentNullException(nameof(decoratedValidator));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }
  
  public async Task<ValidationResult> ValidateAsync(T objectToValidate, CancellationToken cancellationToken = default)
  {
    if (objectToValidate == null) throw new ArgumentNullException(nameof(objectToValidate));

    var objectName = objectToValidate.GetType().Name;
    var validatorName = _decoratedValidator.GetType().Name;

    _logger.Debug("{Validator} validating {ValidationObject}: {@ValidationObjectJson}", 
      validatorName, objectName, objectToValidate);

    ValidationResult result = null;
    var sw = Stopwatch.StartNew();
    try
    {
      result = await _decoratedValidator.ValidateAsync(objectToValidate, cancellationToken);
      sw.Stop();
      _logger.Debug("{Validator} validated {ValidationObject} in {ValidationTime} msec, result is: {@ValidationResult}", 
        validatorName, objectName, sw.ElapsedMilliseconds, result);
    }
    catch (Exception ex)
    {
      sw.Stop();
      _logger.Error("{Validator} failed validating {ValidationObject} after {ValidationTime} msec", 
        ex, validatorName, objectName, sw.ElapsedMilliseconds);
      throw;
    }

    return result;
  }
}