using System.Reflection;
using CqrsFramework.Event;
using SimpleInjector;

namespace CqrsFramework.Fluent;

public class EventHandlerBatchConfigurationBuilder
{
    private readonly Container _container;
    private readonly IList<Assembly> _assemblies;
    private readonly CqrsConfigurationBuilder _parentBuilder;

    public EventHandlerBatchConfigurationBuilder(CqrsConfigurationBuilder parentBuilder, Container container,
        IList<Assembly> assemblies)
    {
        _container = container ?? throw new ArgumentNullException(nameof(container));
        _assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
        _parentBuilder = parentBuilder ?? throw new ArgumentNullException(nameof(parentBuilder));

        RegisterHandlers();
    }

    private void RegisterHandlers()
    {
        _container.Collection.Register(typeof(IEventHandler<>), _assemblies);
    }

    public EventHandlerBatchConfigurationBuilder UseCompositeHandler()
    {
        _container.Register(typeof(IEventHandler<>), typeof(CompositeEventHandler<>));
        return this;
    }

    public EventHandlerBatchConfigurationBuilder DecorateWith(Type decoratorType)
    {
        _container.RegisterDecorator(typeof(IEventHandler<>), decoratorType);
        return this;
    }

    public EventHandlerBatchConfigurationBuilder DecorateWith(Type decoratorType, Lifestyle lifestyle)
    {
        _container.RegisterDecorator(typeof(IEventHandler<>), decoratorType, lifestyle);
        return this;
    }

    public EventHandlerBatchConfigurationBuilder DecorateWith(Type decoratorType, Predicate<DecoratorPredicateContext> predicate)
    {
        this.DecorateWith(decoratorType, Lifestyle.Transient, predicate);
        return this;
    }

    public EventHandlerBatchConfigurationBuilder DecorateWith(Type decoratorType, Lifestyle lifestyle, Predicate<DecoratorPredicateContext> predicate)
    {
        _container.RegisterDecorator(typeof(IEventHandler<>), decoratorType, lifestyle, predicate);
        return this;
    }
    
    public CqrsConfigurationBuilder And()
    {
        return _parentBuilder;
    }
}