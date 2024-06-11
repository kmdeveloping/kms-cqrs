using System.Diagnostics;
using CqrsFramework.Logging;
using CqrsFramework.Validation;

namespace CqrsFramework.Decorators.Validation;

[DebuggerStepThrough]
public class LoggingValidatorDecorator<T> : IValidator<T>
    where T: class
{
    private readonly IValidator<T> _decoratedValidator;
    private readonly ILogger _logger;

    public LoggingValidatorDecorator(IValidator<T> decoratedValidator, ILogger logger)
    {
        _decoratedValidator = decoratedValidator ?? throw new ArgumentNullException(nameof(decoratedValidator));
        if (logger == null) throw new ArgumentNullException(nameof(logger));
        _logger = logger.ForContext(typeof(LoggingValidatorDecorator<T>));
    }

    public async Task<ValidationResult> ValidateAsync(T objectToValidate, CancellationToken cancellationToken = default)
    {
        if (objectToValidate == null) throw new ArgumentNullException(nameof(objectToValidate));

        var objectName = objectToValidate.GetType().Name;
        var validatorName = _decoratedValidator.GetType().Name;

        using (_logger.PushProperty("ValidationObject", objectToValidate, true))
        {
            _logger.Debug("{ValidatorName} validating {ValidationObjectName}", 
                validatorName, objectName);

            var sw = Stopwatch.StartNew();
            try
            {
                ValidationResult result = await _decoratedValidator.ValidateAsync(objectToValidate, cancellationToken);
                sw.Stop();
                _logger.ForContext("ValidationResult", result, true)
                    .Debug("{ValidatorName} validated {ValidationObjectName} in {ValidationTime} msec, result is: {@IsValid}", 
                    validatorName, objectName, sw.ElapsedMilliseconds, result.IsValid);
                
                return result;
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.Error("{ValidatorName} failed validating {ValidationObjectName} after {ValidationTime} msec", 
                    ex, validatorName, objectName, sw.ElapsedMilliseconds);
                throw;
            }
        }
    }
}