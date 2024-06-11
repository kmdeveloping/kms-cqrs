using System.Diagnostics;
using CqrsFramework.Command;
using CqrsFramework.Event;
using CqrsFramework.Fluent;
using CqrsFramework.Query;
using CqrsFramework.Validation;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace CqrsFramework.DependencyInjection;

[DebuggerStepThrough]
public static class ContainerExtensions
{
    public static CqrsConfigurationBuilder AddCqrs(this Container container)
    {
        if(container == null) throw new ArgumentNullException(nameof(container));
            
        container.RegisterSingleton<IQueryProcessor, DynamicQueryProcessor>();
        container.RegisterSingleton<ICommandProcessor, DynamicCommandProcessor>();
        container.RegisterSingleton<IEventProcessor, DynamicEventProcessor>();
        container.RegisterSingleton<IValidationProcessor, DynamicValidationProcessor>();

        container.RegisterSingleton<ICqrsManager, CqrsManager>();

        return new CqrsConfigurationBuilder(container);
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