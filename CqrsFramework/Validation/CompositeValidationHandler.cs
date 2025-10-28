using System.Diagnostics;
using CqrsFramework.Common;
using CqrsFramework.Logging;

namespace CqrsFramework.Validation;

[DebuggerStepThrough]
public class CompositeValidationHandler<T>(IEnumerable<IValidator<T>> validators, ILogger logger) : IValidator<T> where T: class
{
    private readonly ILogger _logger = logger?.ForContext(typeof(CompositeValidationHandler<>)) ?? throw new ArgumentNullException(nameof(logger));

    public async Task<ValidationResult> ValidateAsync(T objectToValidate, CancellationToken cancellationToken = default)
    {
        var aggregateResult = new ValidationResult();

        if (validators == null || !validators.Any()) return aggregateResult;
        
        foreach (var validator in validators)
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

        return aggregateResult;
    }
}