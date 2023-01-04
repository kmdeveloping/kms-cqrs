using System.Reflection;
using cqrsCore.Command;
using cqrsCore.Configuration;
using cqrsCore.Decorators.Command;
using cqrsCore.Decorators.Event;
using cqrsCore.Decorators.Query;
using cqrsCore.Events;
using cqrsCore.Logging;
using cqrsCore.Query;
using cqrsCore.Validation;
using Serilog;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace cqrsCore.Extensions;

public static class DependencyInjectionServiceExtension
{
  public static CqrsConfigurationBuilder AddCqrs(this Container container)
  {
    if (container is null) throw new ArgumentNullException(nameof(container));
    
    container.RegisterSingleton<IQueryProcessor, DynamicQueryProcessor>();
    container.RegisterSingleton<ICommandProcessor, DynamicCommandProcessor>();
    container.RegisterSingleton<IEventProcessor, DynamicEventProcessor>();
    container.RegisterSingleton<IValidationProcessor, DynamicValidationProcessor>();
    
    container.RegisterSingleton<ICqrsManager, CqrsManager>();
    
    return new CqrsConfigurationBuilder(container);
  }

  public static CqrsConfigurationBuilder AddDefaultCqrs(this CqrsConfigurationBuilder builder, Action<ExtensionOptions> options)
  {
    if (builder is null) throw new ArgumentNullException(nameof(builder));
    if (options is null) throw new ArgumentNullException(nameof(options));

    ExtensionOptions extensionOptions = new ExtensionOptions();
    options.Invoke(extensionOptions);

    builder.AddCommandHandlers(extensionOptions.Assemblies)
      .DecorateWith(typeof(AsyncCommandHandlerDecorator<>))
      .DecorateWith(typeof(TimeoutCommandHandlerDecorator<>))
      .DecorateWith(typeof(RetryCommandHandlerDecorator<>))
      .DecorateWith(typeof(ValidatingCommandHandlerDecorator<>))
      .DecorateWith(typeof(LoggingCommandHandlerDecorator<>))
      .DecorateWith(typeof(LifetimeScopeCommandHandlerProxy<>))
      .And()
      .AddQueryHandlers(extensionOptions.Assemblies)
      .DecorateWith(typeof(TimeoutQueryHandlerDecorator<,>))
      .DecorateWith(typeof(RetryingQueryHandlerDecorator<,>))
      .DecorateWith(typeof(ValidatingQueryHandlerDecorator<,>))
      .DecorateWith(typeof(LoggingQueryHandlerDecorator<,>))
      .DecorateWith(typeof(LifetimeScopeQueryHandlerProxy<,>))
      .And()
      .AddEventHandlers(extensionOptions.Assemblies)
      .UseCompositeHandler()
      .DecorateWith(typeof(AsyncEventHandlerDecorator<>))
      .DecorateWith(typeof(ValidatingEventHandlerDecorator<>))
      .DecorateWith(typeof(LoggingEventHandlerDecorator<>))
      .DecorateWith(typeof(LifetimeScopeEventHandlerProxy<>))
      .And()
      .WithCqrsValidation<DataAnnotationValidator>();

    return builder;
  }

  public static Container AddCqrsCoreLogging(this Container container)
  {
    if (container is null) throw new ArgumentNullException(nameof(container));    
   
    container.Register<cqrsCore.Logging.ILogger, SerilogAdapter>();
    container.Register(typeof(cqrsCore.Logging.ILogger<>), typeof(SerilogAdapter<>));
    container.RegisterInstance<Serilog.ILogger>(Log.Logger);
    
    return container;
  }
  
  public static bool ScopeExists(this Container container)
  {
    var defaultScopedLifestyle = container.Options.DefaultScopedLifestyle;
    Scope scope = defaultScopedLifestyle.GetCurrentScope(container);
    var alreadyExists = (scope != null);
    return alreadyExists;
  }

  public static Scope CreateLifetimeScope(this Container container)
  {
    var defaultScopedLifestyle = container.Options.DefaultScopedLifestyle;
    Scope scope = defaultScopedLifestyle.GetCurrentScope(container);
            
    if (defaultScopedLifestyle is AsyncScopedLifestyle)
      scope = AsyncScopedLifestyle.BeginScope(container);
    else if (defaultScopedLifestyle is ThreadScopedLifestyle)
      scope = ThreadScopedLifestyle.BeginScope(container);
    else
      throw new NotImplementedException($"Scoped lifestyle {defaultScopedLifestyle} is not supported");
        
    return scope;
  }
}