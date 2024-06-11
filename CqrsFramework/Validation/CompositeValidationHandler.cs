using System.Diagnostics;
using CqrsFramework.Common;
using CqrsFramework.Logging;

namespace CqrsFramework.Validation;

[DebuggerStepThrough]
public class CompositeValidationHandler<T> : IValidator<T>
    where T: class
{
    private readonly IEnumerable<IValidator<T>> _validators;
    private readonly ILogger _logger;

    public CompositeValidationHandler(IEnumerable<IValidator<T>> validators, ILogger logger)
    {
        _validators = validators;
        if(logger == null) throw new ArgumentNullException(nameof(logger));
        _logger = logger.ForContext(typeof(CompositeValidationHandler<>));
    }

    public async Task<ValidationResult> ValidateAsync(T objectToValidate, CancellationToken cancellationToken = default)
    {
        ValidationResult aggregateResult = new ValidationResult();
            
        if (_validators != null && _validators.Any())
        {
            foreach (IValidator<T> validator in _validators)
            {
                var validatorName = validator.GetType().GetFriendlyName();
                using (_logger.PushProperty("ValidatorName", validatorName))
                {
                    if (objectToValidate is IValidatable validatable)
                    {
                        if (validatable.DisabledValidators.Any())
                        {
                            if(validatable.DisabledValidators.Contains(validator.GetType().Name))
                                break;
                        }
                    }
                    var result = await validator.ValidateAsync(objectToValidate, cancellationToken);
                    if (result.Messages.Any())
                    {
                        foreach (var msg in result.Messages)
                        {
                            aggregateResult.AddValidationMessage(msg);
                        }
                    }
                    
                    if (!result.IsValid)
                    {
                        // TODO: Add configuration for this behavior
                        // Skip further validation, since validation has already failed
                        break;
                    }
                }
            }
        }

        return aggregateResult;
    }
}