using System.Reflection;
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
  
  public Container Build()
  {
    return _container;
  }
}