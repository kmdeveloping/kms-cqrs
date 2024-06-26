﻿using System.Diagnostics;
using CqrsFramework.Exceptions;
using SimpleInjector;

namespace CqrsFramework.Query;

[DebuggerStepThrough]
public class DynamicQueryProcessor : IQueryProcessor
{
    private readonly Func<Type, object> _handlerFactory;

    public DynamicQueryProcessor(Container container)
    {
        if(container == null) throw new ArgumentNullException(nameof(container));
        _handlerFactory = container.GetInstance;
    }

    public async Task<TResult> ProcessAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
        dynamic handler = _handlerFactory.Invoke(handlerType);
        if (handler == null)
            throw new DependencyNotFoundException(handlerType);

        return await handler.HandleAsync((dynamic) query, cancellationToken);
    }
}