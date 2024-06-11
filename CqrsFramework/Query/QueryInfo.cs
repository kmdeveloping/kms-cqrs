using System.Diagnostics;

namespace CqrsFramework.Query;

[DebuggerDisplay("{QueryType.Name,nq}")]
public sealed class QueryInfo
{
    public readonly Type QueryType;
    public readonly Type ResultType;

    public QueryInfo(Type queryType)
    {
        QueryType = queryType;
        ResultType = DetermineResultType(queryType).Single();
    }

    public static bool IsQuery(Type type) => DetermineResultType(type).Any();

    private static IEnumerable<Type> DetermineResultType(Type type) =>
        from interfaceType in type.GetInterfaces()
        where interfaceType.IsGenericType
        where interfaceType.GetGenericTypeDefinition() == typeof(IQuery<>)
        select interfaceType.GetGenericArguments()[0];
}