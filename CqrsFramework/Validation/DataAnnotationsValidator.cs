using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace CqrsFramework.Validation;

[DebuggerStepThrough]
public class DataAnnotationsValidator : IValidator
{
    public Task ValidateAsync(object objectToValidate, CancellationToken cancellationToken = default)
    {
        var context = new ValidationContext(objectToValidate, null, null);
        Validator.ValidateObject(objectToValidate, context, true);
        return Task.CompletedTask;
    }
}