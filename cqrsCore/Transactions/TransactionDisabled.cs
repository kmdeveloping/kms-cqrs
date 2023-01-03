namespace cqrsCore.Transactions;

[AttributeUsage(AttributeTargets.Class|AttributeTargets.Method)]
public class TransactionDisabled : System.Attribute
{
}