using System.Diagnostics;

namespace CqrsFramework.Query;

[DebuggerDisplay("{QueryType.Name,nq}")]
public sealed class QueryInfo(Type queryType)
{
    public readonly Type QueryType = queryType;
    public readonly Type ResultType = DetermineResultType(queryType).Single();

    public static bool IsQuery(Type type) => DetermineResultType(type).Any();

    private static IEnumerable<Type> DetermineResultType(Type type) =>
        from interfaceType in type.GetInterfaces()
        where interfaceType.IsGenericType
        where interfaceType.GetGenericTypeDefinition() == typeof(IQuery<>)
        select interfaceType.GetGenericArguments()[0];
}