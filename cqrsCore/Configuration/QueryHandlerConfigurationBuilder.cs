using System.Reflection;
using cqrsCore.Query;
using SimpleInjector;

namespace cqrsCore.Configuration;

public class QueryHandlerConfigurationBuilder
{
  private readonly Container _container;
  private readonly IList<Assembly> _assemblies;
  private readonly CqrsConfigurationBuilder _parentBuilder;

  public QueryHandlerConfigurationBuilder(CqrsConfigurationBuilder parentBuilder, Container container,
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

  public QueryHandlerConfigurationBuilder DecorateWith(Type decoratorType)
  {
    _container.RegisterDecorator(typeof(IQueryHandler<,>), decoratorType);
    return this;
  }

  public QueryHandlerConfigurationBuilder DecorateWith(Type decoratorType, Lifestyle lifestyle)
  {
    _container.RegisterDecorator(typeof(IQueryHandler<,>), decoratorType, lifestyle);
    return this;
  }

  public QueryHandlerConfigurationBuilder DecorateWith(Type decoratorType, Predicate<DecoratorPredicateContext> predicate)
  {
    DecorateWith(decoratorType, Lifestyle.Transient, predicate);
    return this;
  }

  public QueryHandlerConfigurationBuilder DecorateWith(Type decoratorType, Lifestyle lifestyle, Predicate<DecoratorPredicateContext> predicate)
  {
    _container.RegisterDecorator(typeof(IQueryHandler<,>), decoratorType, lifestyle, predicate);
    return this;
  }

  public QueryHandlerConfigurationBuilder AddPaginationHandler()
  {
    _container.Register(typeof(IQueryHandler<,>), typeof(PagedQueryHandler<,>));
    return this;
  }

  public CqrsConfigurationBuilder And()
  {
    return _parentBuilder;
  }
}