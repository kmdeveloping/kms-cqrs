using System.Reflection;
using CqrsFramework.Query;
using CqrsFramework.RateLimiting;
using SimpleInjector;

namespace CqrsFramework.Fluent;

public class QueryHandlerBatchConfigurationBuilder
{
    private readonly Container _container;
    private readonly IList<Assembly> _assemblies;
    private readonly CqrsConfigurationBuilder _parentBuilder;

    public QueryHandlerBatchConfigurationBuilder(CqrsConfigurationBuilder parentBuilder, Container container,
        IList<Assembly> assemblies)
    {
        _container = container ?? throw new ArgumentNullException(nameof(container));
        _assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
        _parentBuilder = parentBuilder ?? throw new ArgumentNullException(nameof(parentBuilder));

        RegisterHandlers();
    }

    private void RegisterHandlers()
    {
        _container.Register(typeof(IQueryHandler<,>), _assemblies);
    }

    public QueryHandlerBatchConfigurationBuilder DecorateWith(Type decoratorType)
    {
        _container.RegisterDecorator(typeof(IQueryHandler<,>), decoratorType);
        return this;
    }

    public QueryHandlerBatchConfigurationBuilder DecorateWith(Type decoratorType, Lifestyle lifestyle)
    {
        _container.RegisterDecorator(typeof(IQueryHandler<,>), decoratorType, lifestyle);
        return this;
    }

    public QueryHandlerBatchConfigurationBuilder DecorateWith(Type decoratorType, Predicate<DecoratorPredicateContext> predicate)
    {
        DecorateWith(decoratorType, Lifestyle.Transient, predicate);
        return this;
    }

    public QueryHandlerBatchConfigurationBuilder DecorateWith(Type decoratorType, Lifestyle lifestyle, Predicate<DecoratorPredicateContext> predicate)
    {
        _container.RegisterDecorator(typeof(IQueryHandler<,>), decoratorType, lifestyle, predicate);
        return this;
    }

    public QueryHandlerBatchConfigurationBuilder WithRateLimitingConfiguration(RateLimiterConstraints constraints)
    {
        if(constraints != null)
            _container.RegisterInstance(constraints);

        return this;
    }

    public QueryHandlerBatchConfigurationBuilder AddPaginationHandler()
    {
        _container.Register(typeof(IQueryHandler<,>), typeof(PagedQueryHandler<,>));
        return this;
    }

    public CqrsConfigurationBuilder And()
    {
        return _parentBuilder;
    }
}