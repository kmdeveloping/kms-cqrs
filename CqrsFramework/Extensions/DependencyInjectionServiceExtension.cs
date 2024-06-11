using CqrsFramework.Command;
using CqrsFramework.Configuration;
using CqrsFramework.Decorators.Command;
using CqrsFramework.Decorators.Event;
using CqrsFramework.Decorators.Query;
using CqrsFramework.Decorators.Validation;
using CqrsFramework.Events;
using CqrsFramework.Logging;
using CqrsFramework.Query;
using CqrsFramework.Validation;
using Serilog;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using ILogger = CqrsFramework.Logging.ILogger;

namespace CqrsFramework.Extensions;

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

  public static Container AddDefaultConfigurations(this CqrsConfigurationBuilder builder, Action<ExtensionOptions> options)
  {
    if (builder is null) throw new ArgumentNullException(nameof(builder));
    if (options is null) throw new ArgumentNullException(nameof(options));

    ExtensionOptions extensionOptions = new ExtensionOptions();
    options.Invoke(extensionOptions);

    builder
      .AddCommandHandlers(extensionOptions.Assemblies)
      .DecorateWith(typeof(ValidatingCommandHandlerDecorator<>))
      .DecorateWith(typeof(LoggingCommandHandlerDecorator<>))
      .DecorateWith(typeof(LifetimeScopeCommandHandlerProxy<>))
      .And()
      .AddQueryHandlers(extensionOptions.Assemblies)
      .DecorateWith(typeof(ValidatingQueryHandlerDecorator<,>))
      .DecorateWith(typeof(LoggingQueryHandlerDecorator<,>))
      .DecorateWith(typeof(LifetimeScopeQueryHandlerProxy<,>))
      .And()
      .AddValidators(extensionOptions.Assemblies)
      .DecorateWith(typeof(LoggingValidatorDecorator<>))
      .And()
      .AddEventHandlers(extensionOptions.Assemblies)
      .UseCompositeHandler()
      .DecorateWith(typeof(ValidatingEventHandlerDecorator<>))
      .DecorateWith(typeof(LoggingEventHandlerDecorator<>))
      .DecorateWith(typeof(LifetimeScopeEventHandlerProxy<>))
      .And()
      .WithCqrsValidation<DataAnnotationValidator>();

    return builder.Build();
  }

  public static Container AddCqrsCoreLogging(this Container container)
  {
    if (container is null) throw new ArgumentNullException(nameof(container));    
   
    container.Register<ILogger, SerilogAdapter>();
    container.Register(typeof(ILogger<>), typeof(SerilogAdapter<>));
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