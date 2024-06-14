using System.Reflection;
using System.Text.RegularExpressions;
using CqrsFramework.Command;
using CqrsFramework.Event;
using CqrsFramework.Query;
using CqrsFramework.Validation;

namespace CqrsFramework;

public static class BootstrapHelper
{
    public static List<Assembly> DiscoverContractAssemblies(string? searchDirectoryPath = null)
    {
        if (string.IsNullOrEmpty(searchDirectoryPath)) searchDirectoryPath = AppContext.BaseDirectory;

        var assemblies =
            from file in new DirectoryInfo(searchDirectoryPath).GetFiles()
            where file.Extension.ToLower() == ".dll"
            select Assembly.Load(AssemblyName.GetAssemblyName(file.FullName));
        
        var contractAssemblies = (
                from assembly in assemblies
                from type in GetContractTypes(assembly)
                select assembly
            )
            .Distinct()
            .ToList();
        
        return contractAssemblies;
    }

    /// <summary>
    /// Scans assemblies in the specified path for exported types that implement any of the following interfaces:
    ///     <see cref="ICommandHandler{TCommand}"/>
    ///     <see cref="IQueryHandler{TQuery,TResult}"/>
    ///     <see cref="IEventHandler{TEvent}"/>
    /// </summary>
    /// <param name="searchDirectoryPath">The directory path to search. If null, will search in: <see cref="AppContext.BaseDirectory"/>.</param>
    /// <returns>List of assemblies found that implement one of the handler interfaces.</returns>
    public static List<Assembly> DiscoverHandlerAssemblies(string? searchDirectoryPath = null)
    {
        if (string.IsNullOrEmpty(searchDirectoryPath)) searchDirectoryPath = AppContext.BaseDirectory;

        var dllAssemblies =
            from file in new DirectoryInfo(searchDirectoryPath!).GetFiles()
            where file.Extension.ToLower() == ".dll"
            where file.Name.Contains("handlers", StringComparison.OrdinalIgnoreCase)
            let assembly = Assembly.Load(AssemblyName.GetAssemblyName(file.FullName))
            where assembly.GetName().Name.EndsWith("Handlers")
            select assembly;
        
        var handlerAssemblies = (
                from assembly in dllAssemblies
                from type in assembly.GetExportedTypes()
                where
                    type.Name.EndsWith("Handler") || 
                    type.Name.EndsWith("Handlers")
                from intf in type.GetInterfaces()
                where intf.IsGenericType
                let intfType = intf.GetGenericTypeDefinition()
                where
                    intfType == typeof(ICommandHandler<>) ||
                    intfType == typeof(IQueryHandler<,>) ||
                    intfType == typeof(IEventHandler<>)
                // where 
                //     typeof(ICommandHandler<>).IsAssignableFrom(type) ||
                //     typeof(IQueryHandler<,>).IsAssignableFrom(type) ||
                //     typeof(IEventHandler<>).IsAssignableFrom(type)
                select assembly
            )
            .Distinct()
            .ToList();

        return handlerAssemblies;
    }
    
    public static List<Assembly> GetValidatorAssemblies(string? searchDirectoryPath = null)
    {
        if(string.IsNullOrEmpty(searchDirectoryPath))
            searchDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
        
        var dllAssemblies =
            from file in new DirectoryInfo(searchDirectoryPath).GetFiles()
            where file.Extension.ToLower() == ".dll"
            select Assembly.Load(AssemblyName.GetAssemblyName(file.FullName));
        
        var validatorAssemblies = (
            from assy in dllAssemblies
            from type in assy.GetExportedTypes()
            from intf in type.GetInterfaces()
            where type.Name.EndsWith("Validator")
            where
                intf.IsAssignableFrom(typeof(IValidator)) ||
                intf.IsAssignableFrom(typeof(IValidator<>))
            select assy
        ).ToList();

        return validatorAssemblies;
    }
    
    public static IEnumerable<Type> GetKnownCommandTypes(IEnumerable<Assembly> contractAssemblies) =>
        from assembly in contractAssemblies
        from type in GetContractTypes(assembly)
        where !type.IsAbstract && !type.IsInterface
        where typeof(ICommand).IsAssignableFrom(type)
        select type;

    // public static IEnumerable<QueryInfo> GetKnownQueryTypes(IEnumerable<Assembly> contractAssemblies) =>
    //     from assembly in contractAssemblies
    //     from type in GetContractTypes(assembly)
    //     where !type.IsAbstract && !type.IsInterface
    //     where QueryInfo.IsQuery(type)
    //     select new QueryInfo(type);

    public static IEnumerable<QueryInfo> GetKnownQueryTypes(IEnumerable<Assembly> contractAssemblies)
    {
        var contractTypes =
            from assembly in contractAssemblies
            from type in GetContractTypes(assembly)
            where !type.IsAbstract && !type.IsInterface
            select type;
        
        var queryTypes = 
            from type in contractTypes
            where QueryInfo.IsQuery(type)
            select new QueryInfo(type);
        
        return queryTypes;
    }
        
    
    private static IEnumerable<Type> GetContractTypes(Assembly assembly)
    {
        return
            from type in assembly.GetExportedTypes()
            where 
                !type.IsAbstract && !type.IsInterface
            where
                typeof(ICommand).IsAssignableFrom(type) ||
                typeof(IEvent).IsAssignableFrom(type) ||
                //typeof(IQuery<>).IsAssignableFrom(type) ||
                IsOfType(type, typeof(IQuery<>))
                
            select type;
    }

    private static bool IsOfType(Type type, Type comparisonType)
    {
        return (
            from interfaceType in type.GetInterfaces()
            where interfaceType.IsGenericType
            where !type.IsAbstract
            where !type.IsInterface
            where !type.IsGenericTypeDefinition
            where interfaceType.GetGenericTypeDefinition() == comparisonType
            select type
        ).Any();   
    }
        
     // private static IEnumerable<Type> DetermineResultType(Type type) =>
     //     from interfaceType in type.GetInterfaces()
     //     where interfaceType.IsGenericType
     //     where interfaceType.GetGenericTypeDefinition() == typeof(IQuery<>)
     //     select interfaceType.GetGenericArguments()[0];
}