using System.ComponentModel.DataAnnotations;

namespace CqrsFramework.Validation;

public class DataAnnotationValidator : IValidator
{
  public DataAnnotationValidator()
  {
  }

  public Task ValidateAsync(object objectToValidate, CancellationToken cancellationToken = default)
  { 
    var context = new ValidationContext(objectToValidate, null, null);
    Validator.ValidateObject(objectToValidate, context, true);
    return Task.CompletedTask;
  }
}