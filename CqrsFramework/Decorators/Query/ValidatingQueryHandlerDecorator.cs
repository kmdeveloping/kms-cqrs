using System.Diagnostics;
using CqrsFramework.Query;
using CqrsFramework.Validation;

namespace CqrsFramework.Decorators.Query;

[DebuggerStepThrough]
public class ValidatingQueryHandlerDecorator<TQuery, TResult>(IValidator validator, IQueryHandler<TQuery, TResult> decoratedHandler) : IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    private readonly IValidator _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    private readonly IQueryHandler<TQuery,TResult> _decoratedHandler = decoratedHandler ?? throw new ArgumentNullException(nameof(decoratedHandler));

    public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
    {
        if (query == null) throw new ArgumentNullException(nameof(query));

        await _validator.ValidateAsync(query, cancellationToken);
            
        return await _decoratedHandler.HandleAsync(query, cancellationToken);
    }
}