using CqrsFramework.Query;
using ExampleMvc.QueryContracts;

namespace ExampleMvc.QueryHandlers;

public class ExampleQueryHandler : IQueryHandler<ExampleQuery, string>
{
    public Task<string> HandleAsync(ExampleQuery query, CancellationToken cancellationToken = default)
    {
        query.Result = AddEmailToName(query.Name);
        return Task.FromResult(query.Result);
    }
    
    private string AddEmailToName(string name)
    {
        return $"{name}@some-email.com";
    }
}