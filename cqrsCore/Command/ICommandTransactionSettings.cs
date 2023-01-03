namespace cqrsCore.Command;

public interface ICommandTransactionSettings
{
  bool TransactionsEnabled { get; }
  int TransactionTimeoutMinutes { get; }
}

public class CommandTransactionSettings : ICommandTransactionSettings
{
  public virtual bool TransactionsEnabled { get; set; }
  public virtual int TransactionTimeoutMinutes { get; set; }
}