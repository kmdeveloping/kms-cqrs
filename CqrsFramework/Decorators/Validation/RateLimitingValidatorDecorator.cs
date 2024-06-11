using System.Diagnostics;
using CqrsFramework.Common;
using CqrsFramework.Logging;
using CqrsFramework.RateLimiting;
using CqrsFramework.Validation;
using RateLimiter;

namespace CqrsFramework.Decorators.Validation;

[DebuggerStepThrough]
public class RateLimitingValidatorDecorator<T> : IValidator<T>
    where T: class
{
    private readonly IValidator<T> _decoratedValidator;
    private readonly ILogger _logger;
    private readonly RateLimiter.TimeLimiter? _rateLimiter;
    private readonly RateLimiterConstraints _constraints;
    private readonly Type _validatorType;

    public RateLimitingValidatorDecorator(IValidator<T> decoratedValidator, ILogger logger, RateLimiterConstraints constraints)
    {
        _decoratedValidator = decoratedValidator ?? throw new ArgumentNullException(nameof(decoratedValidator));
        if (logger == null) throw new ArgumentNullException(nameof(logger));
        _logger = logger.ForContext(typeof(RateLimitingValidatorDecorator<T>));
        _constraints = constraints ?? throw new ArgumentNullException(nameof(constraints));

        _validatorType = _decoratedValidator.GetType();

        if(IsValidatorConfigured())
            _rateLimiter = TimeLimiter.Compose(_constraints[_validatorType].ToArray());
    }

    private bool IsValidatorConfigured()
    {
        return (_constraints.HasKey(_validatorType));
    }

    public async Task<ValidationResult> ValidateAsync(T objectToValidate, CancellationToken cancellationToken = default)
    {
        // NOTE: Only use rate limiter for commands which are configured...
        if (IsValidatorConfigured() && _rateLimiter != null)
        {
            var objectName = objectToValidate.GetType().Name;
            var validatorName = _decoratedValidator.GetType().Name;
            
            _logger.Debug("Executing validator {ValidatorName} on {ValidationObjectName} with rate-limiting", 
                validatorName, objectName);
                
            return await _rateLimiter.Enqueue(async () => 
                await _decoratedValidator.ValidateAsync(objectToValidate, cancellationToken), cancellationToken);
        }
        else
        {
            return await _decoratedValidator.ValidateAsync(objectToValidate, cancellationToken);
        }
    }
}