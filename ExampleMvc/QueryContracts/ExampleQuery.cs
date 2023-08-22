using cqrsCore.Query;

namespace ExampleMvc.QueryContracts;

public class ExampleQuery : IQuery<string>
{
    public string Name { get; set; }
    public string Result { get; set; }
}