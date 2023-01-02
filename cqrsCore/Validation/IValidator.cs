namespace cqrsCore.Validation;

public interface IValidator
{
  Task ValidateAsync(object objectToValidate, CancellationToken cancellationToken = default);
}

public interface IValidator<T> where T : class
{
  Task<ValidationResult> ValidateAsync(T objectToValidate, CancellationToken cancellationToken = default);
}