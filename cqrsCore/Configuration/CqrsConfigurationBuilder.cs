using System.Reflection;
using cqrsCore.Validation;
using SimpleInjector;

namespace cqrsCore.Configuration;

public class CqrsConfigurationBuilder
{
  private readonly Container _container;

  public CqrsConfigurationBuilder(Container container)
  {
    _container = container ?? throw new ArgumentNullException(nameof(container));
  }

  public CommandHandlerConfigurationBuilder AddCommandHandlers(IList<Assembly> assemblies)
  {
    return new CommandHandlerConfigurationBuilder(this, _container, assemblies);
  }

  public QueryHandlerConfigurationBuilder AddQueryHandlers(IList<Assembly> assemblies)
  {
    return new QueryHandlerConfigurationBuilder(this, _container, assemblies);
  }

  public EventHandlerConfigurationBuilder AddEventHandlers(IList<Assembly> assemblies)
  {
    return new EventHandlerConfigurationBuilder(this, _container, assemblies);
  }
  
  public ValidatorConfigurationBuilder AddValidators(IList<Assembly> assemblies, IList<string> disabledValidators = null)
  {
    return new ValidatorConfigurationBuilder(this, _container, assemblies, disabledValidators);
  }
  
  public CqrsConfigurationBuilder WithCqrsValidation<TValidator>() where TValidator : class, IValidator
  {
    _container.RegisterSingleton<IValidator, TValidator>();
    return this;
  }
  
  public Container Build()
  {
    return _container;
  }
}