namespace CqrsFramework.Validation;

public interface IValidationProcessor
{
  /// <summary>
  /// Invokes the validator for the specified object.
  /// </summary>
  /// <param name="obj">The object to validate.</param>
  /// <param name="cancellationToken"></param>
  Task<ValidationResult> ProcessValidationAsync<T>(T obj, CancellationToken cancellationToken = default);
}