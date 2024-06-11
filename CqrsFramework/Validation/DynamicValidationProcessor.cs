using System.Diagnostics;
using CqrsFramework.Exceptions;
using SimpleInjector;

namespace CqrsFramework.Validation;

[DebuggerStepThrough]
public class DynamicValidationProcessor : IValidationProcessor
{
    private readonly Func<Type, object> _handlerFactory;

    public DynamicValidationProcessor(Container container)
    {
        if(container == null) throw new ArgumentNullException(nameof(container));
        _handlerFactory = container.GetInstance;
    }

    /// <summary>
    /// Invokes the validator for the specified object.
    /// </summary>
    /// <param name="obj">The object to validate.</param>
    /// <param name="cancellationToken"></param>
    public async Task<ValidationResult> ProcessValidationAsync<T>(T obj, CancellationToken cancellationToken = default)
    {
        var validatorType = typeof(IValidator<>).MakeGenericType(obj.GetType());
        dynamic handler = _handlerFactory.Invoke(validatorType);
        if(handler == null)
            throw new DependencyNotFoundException(validatorType);

        return await handler.ValidateAsync((dynamic) obj, cancellationToken);
    }
}