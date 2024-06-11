using CqrsFramework.Query;
using CqrsFramework.Validation;

namespace CqrsFramework.Decorators.Query;

public class ValidatingQueryHandlerDecorator<TQuery,TResult> : IQueryHandler<TQuery,TResult>
  where TQuery: IQuery<TResult>
{
  private readonly IValidator _validator;
  private readonly IQueryHandler<TQuery,TResult> _decoratedHandler;

  public ValidatingQueryHandlerDecorator(IValidator validator, IQueryHandler<TQuery,TResult> decoratedHandler)
  {
    _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    _decoratedHandler = decoratedHandler ?? throw new ArgumentNullException(nameof(decoratedHandler));
  }

  public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
  {
    if (query == null) throw new ArgumentNullException(nameof(query));

    await _validator.ValidateAsync(query, cancellationToken);
            
    return await _decoratedHandler.HandleAsync(query, cancellationToken);
  }
}