using cqrsCore.Command;
using cqrsCore.Configuration;
using SimpleInjector;

namespace cqrsCore.Extensions;

public static class DependencyInjectionServiceExtension
{
  public static CqrsConfigurationBuilder AddCqrs(this Container container)
  {
    if (container is null) throw new ArgumentNullException(nameof(container));
    
    container.RegisterSingleton<ICommandProcessor, DynamicCommandProcessor>();
    
    container.RegisterSingleton<ICqrsManager, CqrsManager>();
    
    return new CqrsConfigurationBuilder(container);
  }
}