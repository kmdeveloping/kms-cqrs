using System.Reflection;
using cqrsCore.Command;
using SimpleInjector;

namespace cqrsCore.Configuration;

public class CommandHandlerConfigurationBuilder
{
  private readonly Container _container;
  private readonly IList<Assembly> _assemblies;
  private readonly CqrsConfigurationBuilder _parentBuilder;

  public CommandHandlerConfigurationBuilder(CqrsConfigurationBuilder parentBuilder, Container container, IList<Assembly> assemblies)
  {
    _container = container ?? throw new ArgumentNullException(nameof(container));
    _assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
    _parentBuilder = parentBuilder ?? throw new ArgumentNullException(nameof(parentBuilder));
    
    RegisterHandlers();
  }

  private void RegisterHandlers()
  {
    _container.Register(typeof(ICommandHandler<>), _assemblies);
  }

  public CommandHandlerConfigurationBuilder DecorateWith(Type decoratorType)
  {
    _container.RegisterDecorator(typeof(ICommandHandler<>), decoratorType);
    return this;
  }

  public CommandHandlerConfigurationBuilder DecorateWith(Type decoratorType, Lifestyle lifestyle)
  {
    _container.RegisterDecorator(typeof(ICommandHandler<>), decoratorType, lifestyle);
    return this;
  }
  
  public CommandHandlerConfigurationBuilder DecorateWith(Type decoratorType, Predicate<DecoratorPredicateContext> predicate)
  {
    DecorateWith(decoratorType, Lifestyle.Transient, predicate);
    return this;
  }

  public CommandHandlerConfigurationBuilder DecorateWith(Type decoratorType, Lifestyle lifestyle, Predicate<DecoratorPredicateContext> predicate)
  {
    _container.RegisterDecorator(typeof(ICommandHandler<>), decoratorType, lifestyle, predicate);
    return this;
  }

  public CqrsConfigurationBuilder And()
  {
    return _parentBuilder;
  }
}